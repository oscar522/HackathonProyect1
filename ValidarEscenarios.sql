
select delivery_date dia, 
	count(1) ordenes, 
	count(distinct track_number) camiones, 
	max(track_order) maxOrdenesXCamion, 
	sum(distance) distancia, 
	sum(distance)/count(1) distanciaPromedio
from Orders
where delivery_date is not null
group by delivery_date
order by 1



select * from Orders where delivery_date='20210915' and track_number =250
order by track_order