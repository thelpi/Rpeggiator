drop table if exists gate_trigger;
drop table if exists rift;
drop table if exists permanent_structure;
drop table if exists door;
drop table if exists chest;
drop table if exists enemy;
drop table if exists enemy_step;
drop table if exists pit;
drop table if exists floor;
drop table if exists pickable_item;
drop table if exists gate;
drop table if exists screen;

create table gate_trigger (
	id int not null,
	screen_id int not null,
	x float not null,
	y float not null,
	width float not null,
	height float not null,
	render_type int not null,
	render_value Varchar(4095) not null,
	action_duration float not null,
	gate_id int not null,
	appear_on_activation int not null,
	on_render_type int not null,
	on_render_value Varchar(4095) not null
);

create table rift (
	id int not null,
	screen_id int not null,
	x float not null,
	y float not null,
	width float not null,
	height float not null,
	render_type int not null,
	render_value Varchar(4095) not null,
	lifepoints float not null
);

create table permanent_structure (
	id int not null,
	screen_id int not null,
	x float not null,
	y float not null,
	width float not null,
	height float not null,
	render_type int not null,
	render_value Varchar(4095) not null
);

create table door (
	id int not null,
	screen_id int not null,
	x float not null,
	y float not null,
	width float not null,
	height float not null,
	render_type int not null,
	render_value Varchar(4095) not null,
	key_id int null,
	connected_screen_id int not null,
	player_go_through_x float not null,
	player_go_through_y float not null,
	locked_render_type int not null,
	locked_render_value Varchar(4095) not null
);

create table chest (
	id int not null,
	screen_id int not null,
	x float not null,
	y float not null,
	width float not null,
	height float not null,
	render_type int not null,
	render_value Varchar(4095) not null,
	item_type int null,
	quantity int not null,
	key_id int null,
	key_id_container int null,
	open_render_type int not null,
	open_render_value Varchar(4095) not null
);

create table enemy (
	id int not null,
	screen_id int not null,
	x float not null,
	y float not null,
	width float not null,
	height float not null,
	maximal_life_points float not null,
	hit_life_point_cost float not null,
	speed float not null,
	recovery_time float not null,
	render_filename Varchar(255) not null,
	render_recovery_filename Varchar(255) not null,
	default_direction int not null,
	loot_item_type int null,
	loot_quantity int not null
);

create table enemy_step (
	enemy_id int not null,
	step_no int not null,
	x float not null,
	y float not null
);

create table pit (
	id int not null,
	screen_id int not null,
	x float not null,
	y float not null,
	width float not null,
	height float not null,
	render_type int not null,
	render_value Varchar(4095) not null,
	screen_id_entrance int null
);

create table floor (
	id int not null,
	screen_id int not null,
	x float not null,
	y float not null,
	width float not null,
	height float not null,
	render_type int not null,
	render_value Varchar(4095) not null,
	floor_type int not null
);

create table pickable_item (
	id int not null,
	screen_id int not null,
	x float not null,
	y float not null,
	width float not null,
	height float not null,
	item_type int null,
	quantity int not null,
	time_before_disapear float null
);

create table gate (
	id int not null,
	screen_id int not null,
	x float not null,
	y float not null,
	width float not null,
	height float not null,
	render_type int not null,
	render_value Varchar(4095) not null,
	activated int not null
);

create table screen (
	id int not null,
	x float not null,
	y float not null,
	width float not null,
	height float not null,
	render_type int not null,
	render_value Varchar(4095) not null,
	floor_type int not null,
	darkness_opacity float not null,
	neighboring_screen_top int not null,
	neighboring_screen_bottom int not null,
	neighboring_screen_right int not null,
	neighboring_screen_left int not null
);