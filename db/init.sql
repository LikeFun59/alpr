if not exists(select * from sys.databases where name = 'lpdb') begin create database lpdb end
go
use lpdb

if not exists(select 1 from sys.tables where name = 'main_gate_alpr_license_plates' and type = 'U') begin
    create table main_gate_alpr_license_plates (
        -- заполняется сервером
        id uniqueidentifier primary key,
        license_plate nvarchar(10) not null,
        captured_at datetime not null,

        -- заполняется пользователем
        license_plate_corrected nvarchar(10),
        visitor_name nvarchar(50),
        visitor_company_name nvarchar(50),
        visitor_receiver_name nvarchar(50),
        visit_reason nvarchar(200),
        soft_deleted bit not null default 0,
        drove_away_at datetime,
    ) 
end
go