/*
DROP DATABASE snai_cms
;
*/
CREATE DATABASE snai_cms CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci
;

USE snai_cms
;

CREATE TABLE admins(
	id INT AUTO_INCREMENT PRIMARY KEY,			-- 自增列需为主键
	user_name NVARCHAR(32) NOT NULL,
	`password` VARCHAR(32) NOT NULL,
	role_id INT NOT NULL,
	state SMALLINT NOT NULL DEFAULT 1,			-- 1 启用，2 禁用
	create_time INT NOT NULL DEFAULT 0,
	update_time INT NOT NULL DEFAULT 0,
	last_logon_time INT NOT NULL DEFAULT 0,
	last_logon_ip NVARCHAR(45) not NULL DEFAULT '',
	error_logon_time INT NOT NULL DEFAULT 0,	-- 错误开始时间
	error_logon_count INT NOT NULL DEFAULT 0,	-- 错误次数
	lock_time INT NOT NULL DEFAULT 0 			-- 锁定结束时间
)
;

-- alter table admins add primary key pk_admins (id)  						-- 主键			
ALTER TABLE admins ADD UNIQUE INDEX ix_admins_user_name(user_name)  		-- UNIQUE INDEX 唯一索引
;

-- password:snai2024，保存时加盐md5(盐+password)
INSERT into admins(user_name,`password`,role_id,state,create_time)
VALUES('snai','86a6553dc16e23a7bb34b4306415c206',1,1,1550937600)
;

CREATE TABLE tokens(
	id INT AUTO_INCREMENT PRIMARY KEY,			-- 自增列需为主键
	token NVARCHAR(512) NOT NULL,
	user_id INT NOT NULL,
	state SMALLINT NOT NULL DEFAULT 1,			-- 1 登录，2 退出
	create_time INT NOT NULL DEFAULT 0
)
;
		
ALTER TABLE tokens ADD INDEX ix_tokens_user_id_state(user_id,state)
;

CREATE TABLE modules(
	id INT AUTO_INCREMENT PRIMARY KEY,
	parent_id int not NULL DEFAULT 0,
	title NVARCHAR(32) not NULL,
	router NVARCHAR(64) not NULL DEFAULT '',
	sort int not NULL DEFAULT 1,     			-- 小排在前
	state SMALLINT NOT NULL DEFAULT 1			-- 1 启用，2 禁用
)
;

ALTER TABLE modules ADD UNIQUE INDEX ix_modules_parent_id_title(parent_id,title)  
;		
ALTER TABLE modules ADD INDEX ix_modules_router(router)  			
;

INSERT into modules(id,parent_id,title,router,sort,state)
select 1,-1,'修改密码','/api/home/changepassword',1,1
UNION ALL select 2,-1,'退出','/api/home/logout',1,1
UNION ALL select 3,0,'管理员管理','',10,1
UNION ALL select 4,4,'账号管理','/api/admin/list',11,1
/*
UNION ALL select 5,5,'添加修改账号','ModifyAdmin',11,1
UNION ALL select 6,5,'禁启用账号','UpdateAdminState',11,1
UNION ALL select 7,5,'解锁账号','UnlockAdmin',11,1
UNION ALL select 8,5,'删除账号','DeleteAdmin',11,1
UNION ALL select 9,4,'菜单管理','ModuleList',12,1
UNION ALL select 10,10,'添加修改菜单','ModifyModule',12,1
UNION ALL select 11,10,'禁启用菜单','UpdateModuleState',12,1
UNION ALL select 12,10,'删除菜单','DeleteModule',12,1
UNION ALL select 13,4,'角色管理','RoleList',13,1
UNION ALL select 14,14,'添加修改角色','ModifyRole',13,1
UNION ALL select 15,14,'禁启用角色','UpdateRoleState',13,1
UNION ALL select 16,14,'删除角色','DeleteRole',13,1
UNION ALL select 17,14,'分配权限','ModifyRoleRight',13,1
*/
;

CREATE TABLE roles(
	id int AUTO_INCREMENT PRIMARY KEY,
	title NVARCHAR(32) not NULL,
	state SMALLINT NOT NULL DEFAULT 1			-- 1 启用，2 禁用
)
;

ALTER TABLE roles ADD UNIQUE INDEX ix_roles_title(title)  		
;

INSERT into roles(title,state)
select '超级管理员',1
;

CREATE TABLE role_module(
	id int AUTO_INCREMENT PRIMARY KEY,
	role_id int not NULL,
	module_id int not NULL
)
;

ALTER TABLE role_module ADD UNIQUE INDEX ix_role_module_role_id_module_id(role_id,module_id) 
;

INSERT into role_module(role_id,module_id)
select 1,3
UNION ALL select 1,4
/*
UNION ALL select 1,5
UNION ALL select 1,6
UNION ALL select 1,7
UNION ALL select 1,8
UNION ALL select 1,9
UNION ALL select 1,10
UNION ALL select 1,11
UNION ALL select 1,12
UNION ALL select 1,13
UNION ALL select 1,14
UNION ALL select 1,15
UNION ALL select 1,16
UNION ALL select 1,17
*/
;