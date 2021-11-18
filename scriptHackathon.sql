USE [PruebasAlgoritmo]
GO
/****** Object:  Table [dbo].[DateDelivery]    Script Date: 11/17/2021 9:36:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DateDelivery](
	[dateDelivery_id] [date] NOT NULL,
	[number_Orders] [int] NOT NULL,
 CONSTRAINT [PK_DateDelivery] PRIMARY KEY CLUSTERED 
(
	[dateDelivery_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Orders]    Script Date: 11/17/2021 9:36:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[order_id] [varchar](255) NOT NULL,
	[created_at] [datetime] NOT NULL,
	[delivery_start_date] [date] NOT NULL,
	[delivery_end_date] [date] NOT NULL,
	[latitude] [float] NOT NULL,
	[longitude] [float] NOT NULL,
	[method] [varchar](255) NOT NULL,
	[quantity] [int] NOT NULL,
	[weight] [float] NOT NULL,
	[volume] [float] NOT NULL,
	[date] [date] NOT NULL,
	[delivery_date] [date] NULL,
	[track_number] [int] NULL,
	[track_order] [int] NULL,
	[distance] [float] NULL,
 CONSTRAINT [PK_Orders_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[SelectOrders]    Script Date: 11/17/2021 9:36:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE [dbo].[SelectOrders]
(
    @dateDelivery date,
	@deliveryMin int,
	@deliveryMax int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON
--Valida que la fecha de entrega no se haya procesado
if (select count(1)	from datedelivery where datedelivery_id= @dateDelivery) = 0
begin

----------------------------
--Busca el promedio de entrega de los ultimos días	
----------------------------

	declare @avgDelivery as int
	declare @avgTemp as int

	--Revisa que las cantidades de entrega de los ultimos 8 días estén correctas
	delete from datedelivery where datedelivery_id>dateadd(day,-8, @dateDelivery)

	insert into datedelivery
	select [delivery_date], count(1) 
	from  [dbo].[Orders] 
	where [delivery_date]>dateadd(day,-8, @dateDelivery)
	group by [delivery_date]

	--Calcula el promedio de entrega de los ultimos 7 días
	select @avgDelivery = isnull(AVG(number_Orders),0)
	from (
		select top 7 [number_Orders]
		from datedelivery
		where number_Orders>0
		order by datedelivery_id desc
		) a

	if @avgDelivery<@deliveryMin 
	begin
		set @avgDelivery=@deliveryMin
	end

	if @avgDelivery>@deliveryMax 
	begin
		set @avgDelivery=@deliveryMax
	end
----------------------------
--Selecciona las ordenes a entregar	
----------------------------

	-- Garantiza que no queden entregas pendientes que no se puedan entregar en otro día 
	set @avgTemp = @avgDelivery

	update a
	set delivery_date=@dateDelivery
	from [dbo].[Orders] a
	where [delivery_end_date]<=@dateDelivery and delivery_date is null
	
	set @avgDelivery = @avgDelivery - (select count(1) from [dbo].[Orders] where [delivery_date]=@dateDelivery )

	if @avgDelivery>0 
	begin 
	
		--Selecciona las ordenes a entregar de acuerdo al criterio de promedio de entrega
		/*
		select a.*,b.*
		from [dbo].[Orders] b  --164
		inner join (select * 
			from Orders
			where delivery_date=@dateDelivery
			) a on abs(b.latitude - a.latitude) < 0.00001570000 *5  and abs(b.longitude - a.longitude) <  0.000035300 *5
				and b.[delivery_start_date]<=@dateDelivery and b.delivery_date is null
		*/
	
		update upt
		set delivery_date=@dateDelivery
		from Orders upt
		inner join (
			select top (@avgDelivery) b.id, min(abs(a.longitude-b.longitude) + abs(a.latitude - b.latitude)) dist
			from [dbo].[Orders] b  
			inner join Orders a on b.[delivery_start_date]<=@dateDelivery and b.delivery_date is null and a.delivery_date=@dateDelivery
				--and abs(b.latitude - a.latitude) < 0.00001570000 *50  and abs(b.longitude - a.longitude) <  0.000035300 *50
			group by b.id
			order by 2
			) b on upt.id=b.id

		set @avgDelivery = @avgTemp - (select count(1) from [dbo].[Orders] where [delivery_date]=@dateDelivery )
		if @avgDelivery<0 set @avgDelivery=0

		update a
		set delivery_date=@dateDelivery
		from [dbo].[Orders] a
		inner join(
			select top (@avgDelivery) id
			from [dbo].[Orders]
			where [delivery_start_date]<=@dateDelivery and delivery_date is null
			order by [delivery_end_date], [created_at]
			) b on a.id=b.id

	end

	-- Si todas las ordenes se pueden entregar en otro día y la cantidad de entregas está por debajo del mínimo, entonces no se realizan entregas
	If (select count(1)
		from [dbo].[Orders] a
		where [delivery_date]=@dateDelivery and delivery_end_date=@dateDelivery)=0
		update [dbo].[Orders]
		set [delivery_date]= null where [delivery_date]=@dateDelivery

----------------------------
--Crea la cantidad de entregas del día
----------------------------

	delete from datedelivery where datedelivery_id=@dateDelivery

	insert into datedelivery
	select @dateDelivery, count(1) 
	from  [dbo].[Orders] 
	where delivery_date=@dateDelivery


end

----------------------------
--Retorna ordenes a enviar en la fecha
----------------------------
	select *
	from [dbo].[Orders] a
	where delivery_date=@dateDelivery

END
GO
/****** Object:  StoredProcedure [dbo].[SelectOrders1]    Script Date: 11/17/2021 9:36:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE [dbo].[SelectOrders1]
(
    @dateDelivery date,
	@deliveryMin int,
	@deliveryMax int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON
--Valida que la fecha de entrega no se haya procesado
if (select count(1)	from datedelivery where datedelivery_id= @dateDelivery) = 0
begin

----------------------------
--Busca el promedio de entrega de los ultimos días	
----------------------------

	declare @avgDelivery as int

	--Revisa que las cantidades de entrega de los ultimos 8 días estén correctas
	delete from datedelivery where datedelivery_id>dateadd(day,-8, @dateDelivery)

	insert into datedelivery
	select [delivery_date], count(1) 
	from  [dbo].[Orders] 
	where [delivery_date]>dateadd(day,-8, @dateDelivery)
	group by [delivery_date]

	--Calcula el promedio de entrega de los ultimos 7 días
	select @avgDelivery = isnull(AVG(number_Orders),0)
	from (
		select top 7 [number_Orders]
		from datedelivery
		where number_Orders>0
		order by datedelivery_id desc
		) a

	if @avgDelivery<@deliveryMin 
	begin
		set @avgDelivery=@deliveryMin
	end

	if @avgDelivery>@deliveryMax 
	begin
		set @avgDelivery=@deliveryMax
	end
----------------------------
--Selecciona las ordenes a entregar	
----------------------------

	--Selecciona las ordenes a entregar de acuerdo al criterio de promedio de entrega
	update a
	set delivery_date=@dateDelivery
	from [dbo].[Orders] a
	inner join(
		select top (@avgDelivery) id
		from [dbo].[Orders]
		where [delivery_start_date]<=@dateDelivery and delivery_date is null
		order by [delivery_end_date], [created_at]
		) b on a.id=b.id

	-- Garantiza que no queden entregas pendientes que no se puedan entregar en otro día 
	update a
	set delivery_date=@dateDelivery
	from [dbo].[Orders] a
	where [delivery_end_date]<=@dateDelivery and delivery_date is null

	-- Si todas las ordenes se pueden entregar en otro día y la cantidad de entregas está por debajo del mínimo, entonces no se realizan entregas
	If (select count(1)
		from [dbo].[Orders] a
		where [delivery_date]=@dateDelivery and delivery_end_date=@dateDelivery)=0
		update [dbo].[Orders]
		set [delivery_date]= null where [delivery_date]=@dateDelivery

----------------------------
--Crea la cantidad de entregas del día
----------------------------

	delete from datedelivery where datedelivery_id=@dateDelivery

	insert into datedelivery
	select @dateDelivery, count(1) 
	from  [dbo].[Orders] 
	where delivery_date=@dateDelivery


end

----------------------------
--Retorna ordenes a enviar en la fecha
----------------------------
	select *
	from [dbo].[Orders] a
	where delivery_date=@dateDelivery

END
GO
/****** Object:  StoredProcedure [dbo].[SelectOrders2]    Script Date: 11/17/2021 9:36:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE [dbo].[SelectOrders2]
(
    @dateDelivery date,
	@deliveryMin int,
	@deliveryMax int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON
--Valida que la fecha de entrega no se haya procesado
if (select count(1)	from datedelivery where datedelivery_id= @dateDelivery) = 0
begin

----------------------------
--Busca el promedio de entrega de los ultimos días	
----------------------------

	declare @avgDelivery as int
	declare @avgTemp as int

	--Revisa que las cantidades de entrega de los ultimos 8 días estén correctas
	delete from datedelivery where datedelivery_id>dateadd(day,-8, @dateDelivery)

	insert into datedelivery
	select [delivery_date], count(1) 
	from  [dbo].[Orders] 
	where [delivery_date]>dateadd(day,-8, @dateDelivery)
	group by [delivery_date]

	--Calcula el promedio de entrega de los ultimos 7 días
	select @avgDelivery = isnull(AVG(number_Orders),0)
	from (
		select top 7 [number_Orders]
		from datedelivery
		where number_Orders>0
		order by datedelivery_id desc
		) a

	if @avgDelivery<@deliveryMin 
	begin
		set @avgDelivery=@deliveryMin
	end

	if @avgDelivery>@deliveryMax 
	begin
		set @avgDelivery=@deliveryMax
	end
----------------------------
--Selecciona las ordenes a entregar	
----------------------------

	-- Garantiza que no queden entregas pendientes que no se puedan entregar en otro día 
	set @avgTemp = @avgDelivery

	update a
	set delivery_date=@dateDelivery
	from [dbo].[Orders] a
	where [delivery_end_date]<=@dateDelivery and delivery_date is null
	
	set @avgDelivery = @avgDelivery - (select count(1) from [dbo].[Orders] where [delivery_date]=@dateDelivery )

	if @avgDelivery>0 
	begin 
	
		--Selecciona las ordenes a entregar de acuerdo al criterio de promedio de entrega
		/*
		select a.*,b.*
		from [dbo].[Orders] b  --164
		inner join (select * 
			from Orders
			where delivery_date=@dateDelivery
			) a on abs(b.latitude - a.latitude) < 0.00001570000 *5  and abs(b.longitude - a.longitude) <  0.000035300 *5
				and b.[delivery_start_date]<=@dateDelivery and b.delivery_date is null
		*/
	
		update upt
		set delivery_date=@dateDelivery
		from Orders upt
		inner join (
			select top (@avgDelivery) b.id, min(abs(a.longitude-b.longitude) + abs(a.latitude - b.latitude)) dist
			from [dbo].[Orders] b  
			inner join Orders a on b.[delivery_start_date]<=@dateDelivery and b.delivery_date is null and a.delivery_date=@dateDelivery
				and abs(b.latitude - a.latitude) < 0.00001570000 *50  and abs(b.longitude - a.longitude) <  0.000035300 *50
			group by b.id
			order by 2
			) b on upt.id=b.id

		set @avgDelivery = @avgTemp - (select count(1) from [dbo].[Orders] where [delivery_date]=@dateDelivery )
		if @avgDelivery<0 set @avgDelivery=0

		update a
		set delivery_date=@dateDelivery
		from [dbo].[Orders] a
		inner join(
			select top (@avgDelivery) id
			from [dbo].[Orders]
			where [delivery_start_date]<=@dateDelivery and delivery_date is null
			order by [delivery_end_date], [created_at]
			) b on a.id=b.id

	end

	-- Si todas las ordenes se pueden entregar en otro día y la cantidad de entregas está por debajo del mínimo, entonces no se realizan entregas
	If (select count(1)
		from [dbo].[Orders] a
		where [delivery_date]=@dateDelivery and delivery_end_date=@dateDelivery)=0
		update [dbo].[Orders]
		set [delivery_date]= null where [delivery_date]=@dateDelivery

----------------------------
--Crea la cantidad de entregas del día
----------------------------

	delete from datedelivery where datedelivery_id=@dateDelivery

	insert into datedelivery
	select @dateDelivery, count(1) 
	from  [dbo].[Orders] 
	where delivery_date=@dateDelivery


end

----------------------------
--Retorna ordenes a enviar en la fecha
----------------------------
	select *
	from [dbo].[Orders] a
	where delivery_date=@dateDelivery

END
GO
