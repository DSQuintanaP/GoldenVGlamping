USE master;
GO

-- Verifica si la base de datos existe y elimínala
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'GoldenVGlamping')
    DROP DATABASE GoldenVGlamping;
GO

-- Crea la nueva base de datos
CREATE DATABASE GoldenVGlamping;
GO

-- Cambia el contexto a la nueva base de datos
USE GoldenVGlamping;
GO

create table Permisos (
	IdPermiso int primary key identity(1,1),
	NomPermiso varchar(50)
);

GO

create table Roles (
	IdRol int primary key identity(1,1),
	NomRol varchar(20),
	Estado bit,
);

GO

create table PermisosRoles (
	IdPermisosRoles int primary key identity(1,1),
	IdRol int,
	IdPermiso int,
	foreign key (IdRol) references Roles(IdRol),
	foreign key (IdPermiso) references Permisos(IdPermiso)
);

GO

create table Imagenes (
	IdImagen int primary key identity(1,1),
	UrlImagen nvarchar(max)
);

GO

create table TipoHabitaciones(
	IdTipoHabitacion int primary key identity(1,1),
	NombreTipoHabitacion varchar(20),
	NumeroPersonas int,
	Estado bit,
);

GO

create table Habitaciones(
	IdHabitacion int primary key identity(1,1),
	IdTipoHabitacion int,
	NombreHabitacion varchar(50),
	Estado bit,
	Descripcion varchar(50),
	Costo decimal,

	foreign key (IdTipoHabitacion) references TipoHabitaciones(IdTipoHabitacion)
);

GO

CREATE TABLE Muebles (
    IdMueble INT PRIMARY KEY IDENTITY(1,1),  -- Identificador único del mueble
    NombreMueble VARCHAR(50),                 -- Nombre del mueble (ej. Silla, Mesa, Cama)
    Estado BIT,                               -- Estado del mueble: 1 (disponible), 0 (no disponible)
    Descripcion VARCHAR(100)                  -- Descripción adicional sobre el mueble
);

GO

CREATE TABLE Inventario (
    IdInventario INT PRIMARY KEY IDENTITY(1,1),
    IdHabitacion INT,
    IdMueble INT,
    FOREIGN KEY (IdHabitacion) REFERENCES Habitaciones(IdHabitacion) ON DELETE CASCADE,
    FOREIGN KEY (IdMueble) REFERENCES Muebles(IdMueble)
);

GO

create table ImagenHabitacion(
	IdImagenHabi int primary key identity(1,1),
	IdImagen int,
	IdHabitacion int,

	foreign key (IdImagen) references Imagenes(IdImagen),
	foreign key (IdHabitacion) references Habitaciones(IdHabitacion)
);

GO

create table TipoServicios(
	IdTipoServicio int primary key identity(1,1),
	NombreTipoServicio varchar(50)
);

GO

create table Servicios(
	IdServicio int primary key identity(1,1),
	IdTipoServicio int,
	NomServicio varchar(50),
	Costo decimal, 
	Estado bit,

	foreign key (IdTipoServicio) references TipoServicios(IdTipoServicio)
);

GO

create table ImagenServicio(
	IdImagenServi int primary key identity(1,1),
	IdImagen int,
	IdServicio int,

	foreign key (IdImagen) references Imagenes(IdImagen),
	foreign key (IdServicio) references Servicios(IdServicio)
);

GO

create table Paquetes(
	IdPaquete int primary key identity(1,1),
	NomPaquete varchar(50),
	Costo decimal,
	IdHabitacion int,
	Estado bit,
	Descripcion varchar(200),

	foreign key (IdHabitacion) references Habitaciones (IdHabitacion)

);

GO

create table ImagenPaquete(
	IdImagenPaque int primary key identity(1,1),
	IdImagen int,
	IdPaquete int,

	foreign key (IdImagen) references Imagenes(IdImagen),
	foreign key (IdPaquete) references Paquetes(IdPaquete)
);

GO

create table PaqueteServicio(
	IdPaqueteServicio int primary key identity(1,1),
	IdPaquete int,
	IdServicio int,
	Costo decimal,

	foreign key (IdPaquete) references Paquetes(IdPaquete),
	foreign key (IdServicio) references Servicios(IdServicio)
);

GO

create table TipoDocumento(
	IdTipoDocumento int primary key identity(1,1),
	NomTipoDocumento varchar(50),
);

GO

create table Clientes(
	IdCliente int primary key identity(1,1),
	NroDocumento int,
	IdTipoDocumento int,
	Nombres varchar(20),
	Apellidos varchar(20),
	Celular varchar(12),
	Correo varchar(30),
	Contrasena varchar(15),
	Estado bit,
	IdRol int,

	foreign key (IdRol) references Roles(IdRol),
	foreign key (IdTipoDocumento) references TipoDocumento(IdTipoDocumento)
);

GO

create table Usuarios(
	IdUsuario int primary key identity,
	NroDocumento int,
	IdTipoDocumento int,
	Nombres varchar(20),
	Apellidos varchar(20),
	Celular varchar(12),
	Correo varchar(30),
	Contrasena varchar(15),
	Estado bit,
	IdRol int,

	foreign key (IdRol) references Roles(IdRol),
	foreign key (IdTipoDocumento) references TipoDocumento(IdTipoDocumento)
);

GO

create table MetodoPago(
	IdMetodoPago int primary key identity(1,1),
	NomMetodoPago varchar(20)
);

GO

create table EstadosReserva(
	IdEstadoReserva int primary key identity(1,1),
	NombreEstadoReserva varchar(15)
);

create table Reservas(
	IdReserva int primary key identity(1,1),
	IdCliente int,
	FechaReserva datetime default getdate(),
	FechaInicio datetime,
	FechaFinalizacion datetime,
	SubTotal decimal,
	IVA decimal,
	Descuento decimal,
	MontoTotal decimal,	
	MetodoPago int,
	IdEstadoReserva int default '1'

	foreign key (IdCliente) references Clientes(IdCliente),
	foreign key (IdEstadoReserva) references EstadosReserva(IdEstadoReserva),
	foreign key (MetodoPago) references MetodoPago(IdMetodoPago)
);

GO

create table DetalleReservaServicio (
	IdDetalleReservaServicio int primary key identity(1,1),
	IdServicio int,
	IdReserva int,
	Cantidad int,
	Costo decimal,

	foreign key (IdServicio) references Servicios(IdServicio),
	foreign key (IdReserva) references Reservas(IdReserva)
);

GO

create table DetalleReservaPaquete (
	DetalleReservaPaquete int primary key identity(1,1),
	IdPaquete int,
	IdReserva int,
	Cantidad int,
	Costo decimal,

	foreign key (IdPaquete) references Paquetes(IdPaquete),
	foreign key (IdReserva) references Reservas(IdReserva)
);

GO

create table Abono(
	IdAbono int primary key identity(1,1),
	IdReserva int,
	FechaAbono datetime default getdate(),
	CantAbono decimal,

	foreign key (IdReserva) references Reservas(IdReserva)
);

GO

create table ImagenAbono(
	IdImagenAbono int primary key identity(1,1),
	IdImagen int,
	IdAbono int,

	foreign key (IdImagen) references Imagenes(IdImagen),
	foreign key (IdAbono) references Abono(IdAbono)
);

GO

insert into TipoDocumento(NomTipoDocumento)
values ('C.C'),
	   ('T.I')

GO

insert into Roles(NomRol,Estado)
values ('Administrador',1),
       ('Cliente',1)

GO

insert into TipoHabitaciones(NombreTipoHabitacion,NumeroPersonas,Estado)
values ('Simple',2,1),
       ('Doble',4,1)

GO

insert into Habitaciones(IdTipoHabitacion,NombreHabitacion,Estado,Descripcion,Costo)
values (1,'Habitacion Simple',1,'Habitacion para dos personas',200000),
       (2,'Habitacion Doble',1,'Habitacion para cuatro personas',300000)

GO

insert into TipoServicios(NombreTipoServicio)
values ('Comida'),
	   ('Aire libre')

GO

insert into Servicios(IdTipoServicio,NomServicio,Costo,Estado)
values (1,'Desayuno',30000,1),
	   (1,'Almuerzo',30000,1),
	   (1,'Cena',30000,1),
	   (2,'Cabalgata',100000,1)

GO

insert into MetodoPago(NomMetodoPago)
values ('Efectivo'),
       ('Targeta'),
	   ('DataCuerpo')

GO

insert into EstadosReserva(NombreEstadoReserva)
values ('Reservado'),
       ('Por Confirmar'),
	   ('Confirmado'),
	   ('En Ejecución'),
	   ('Anulado'),
	   ('Finalizado')

GO

insert into Permisos(NomPermiso)
values ('Dashboard'),
	   ('Listar Roles'),
	   ('Buscar Roles'),
	   ('Crear Roles'),
	   ('Ver Detalles Roles'),
	   ('Editar Roles'),
	   ('Cambiar Estado Roles'),
	   ('Listar Usuarios'),
	   ('Buscar Usuarios'),
	   ('Crear Usuarios'),
	   ('Ver Detalles Usuarios'),
	   ('Editar Usuarios'),
	   ('Cambiar Estado Usuarios'),
	   ('Listar Clientes'),
	   ('Buscar Clientes'),
	   ('Crear Clientes'),
	   ('Ver Detalles Clientes'),
	   ('Editar Clientes'),
	   ('Cambiar Estado Clientes'),
	   ('Listar Habitaciones'),
	   ('Buscar Habitaciones'),
	   ('Crear Habitaciones'),
	   ('Ver Detalles Habitaciones'),
	   ('Editar Habitaciones'),
	   ('Cambiar Estado Habitaciones'),
	   ('Listar Tipo Habitaciones'),
	   ('Buscar Tipo Habitaciones'),
	   ('Crear Tipo Habitaciones'),
	   ('Editar Tipo Habitaciones'),
	   ('Cambiar Estado Tipo Habitaciones'),
	   ('Listar Servicios'),
	   ('Buscar Servicios'),
	   ('Crear Servicios'),
	   ('Ver Detalles Servicios'),
	   ('Editar Servicios'),
	   ('Cambiar Estado Servicios'),
	   ('Listar Tipo Servicio'),
	   ('Buscar Tipo Servicios'),
	   ('Crear Tipo Servicios'),
	   ('Editar Tipo Servicios'),
	   ('Listar Paquetes'),
	   ('Buscar Paquetes'),
	   ('Crear Paquetes'),
	   ('Ver Detalles Paquetes'),
	   ('Editar Paquetes'),
	   ('Cambiar Estado Paquetes'),
	   ('Listar Reservas'),
	   ('Buscar Reservas'),
	   ('Crear Reservas'),
	   ('Ver Detalles Reservas'),
	   ('Editar Reservas'),
	   ('Cambiar Estado Reservas'),
	   ('Anular Reserva'),
	   ('Listar Abono'),
	   ('Buscar Abono'),
	   ('Crear Abono'),
	   ('Ver Detalle Abono'),
	   ('Anular Abono')

GO

insert into PermisosRoles(IdRol,IdPermiso)
values	(1,1),
		(1,2),
		(1,3),
		(1,4),
		(1,5),
		(1,6),
		(1,7),
		(1,8),
		(1,9),
		(1,10),
		(1,11),
		(1,12),
		(1,13),
		(1,14),
		(1,15),
		(1,16),
		(1,17),
		(1,18),
		(1,19),
		(1,20),
		(1,21),
		(1,22),
		(1,23),
		(1,24),
		(1,25),
		(1,26),
		(1,27),
		(1,28),
		(1,29),
		(1,30),
		(1,31),
		(1,32),
		(1,33),
		(1,34),
		(1,35),
		(1,36),
		(1,37),
		(1,38),
		(1,39),
		(1,40),
		(1,41),
		(1,42),
		(1,43),
		(1,44),
		(1,45),
		(1,46),
		(1,47),
		(1,48),
		(1,49),
		(1,50),
		(1,51),
		(1,52),
		(1,53),
		(1,54),
		(1,55),
		(1,56),
		(1,57),
		(1,58)