-- ----------------------------
-- Procedure structure for `proc_insert_productaction`
-- ----------------------------
DROP PROCEDURE IF EXISTS `proc_insert_productaction`;
DELIMITER //
CREATE DEFINER=`root`@`localhost` PROCEDURE `proc_insert_productaction`(IN product_number varchar(16),IN parent_number varchar(16),IN product_name varchar(100), IN actionType int, IN actiontime datetime, IN actioncomments varchar(512), IN boxNumber varchar(16), IN boxName varchar(100), IN additionalText varchar(512),IN operators varchar(512) )
BEGIN
    Declare idval int;
    Declare commaCount int;
    Declare i int;
    Declare operatorValue varchar(16);
    Declare productId int; 
    Declare parentId int;
    Declare product_base varchar(2);
    Declare product_year varchar(2);
    Declare product_batch varchar(3);
    Declare product_num varchar(3); 

	Declare boxId int;

    Declare errormsg varchar(512);
    Declare additonalId int;
    
    set errormsg ='';
    
    if product_number  is null then 
		set errormsg =Concat(errormsg, 'Invalid parameter: product_number. '); 
    end if;
  
    if actionType is null then 
		set errormsg =Concat(errormsg, 'Invalid parameter: actionType. ') ;
    end if;

    if actiontime is null then 
		set errormsg =Concat(errormsg, 'Invalid parameter: actiontime. ');
    end if;

    if operators is null then 
		set errormsg =Concat(errormsg,'Invalid parameter: operators. ');
    end if;

    if errormsg='' then 
        SET parentId =0;
        if parent_number is not null and parent_number <>'' then
			set product_base = substring(parent_number ,1,2);
			set product_year = substring(parent_number ,3,2)+0;
			set product_batch = substring(parent_number ,5,3);
			set product_num = substring(parent_number ,8,3);
            SELECT Id FROM products WHERE ProductBaseNumber=product_base and YearNumber=product_year and BatchNumber=product_batch and SerialNumber=product_num into parentId;

            If parentId is null or parentId <=0 then
				INSERT INTO products (Name, ProductBaseNumber, YearNumber, BatchNumber, SerialNumber) Values ('', product_base, product_year, product_batch, product_num);
                select max(Id) from products into parentId ;
            end if;
        end if;

		SET boxId=0;
		if boxNumber is not null and boxNumber <>'' then
			set product_base = substring(boxNumber ,1,2);
			set product_year = substring(boxNumber ,3,2)+0;
			set product_batch = substring(boxNumber ,5,3);
			set product_num = substring(boxNumber ,8,3);
            SELECT Id FROM products WHERE ProductBaseNumber=product_base and YearNumber=product_year and BatchNumber=product_batch and SerialNumber=product_num LIMIT 1 into boxId;

            If boxId is null or boxId <=0 then
				INSERT INTO products (Name, ProductBaseNumber, YearNumber, BatchNumber, SerialNumber) Values (boxName, product_base, product_year, product_batch, product_num);
                select max(Id) from products into boxId ;
            end if;
		end if;

        set product_base = substring(product_number ,1,2);
		set product_year = substring(product_number ,3,2)+0;
		set product_batch = substring(product_number ,5,3);
		set product_num = substring(product_number ,8,3);
        SELECT Id FROM products WHERE ProductBaseNumber=product_base and YearNumber=product_year and BatchNumber=product_batch and SerialNumber=product_num into productId;

        if productId is null or productId<=0 then
			INSERT INTO products (Name, ProductBaseNumber, YearNumber, BatchNumber, SerialNumber) Values (product_name, product_base, product_year, product_batch, product_num);
            select max(Id) from products into productId ;
        else
			if product_name is not null and product_name <>'' then
				Update products set Name = product_name where Id=productId; 
			end if;        
		end if;

        if parentId >0 then
			Update products set ParentId = parentId  where Id=productId; 
        End if;

		if boxId >0 then
			Update products set BoxId = boxId  where Id=productId; 
        End if;

		
        Set additonalId = 0;
        if additionalText is not null and additionalText<>'' then
			INSERT INTO action_additionals (Content) VALUES (additionalText);
            select max(Id) from action_additionals into additonalId ;
        end if;

		Insert Into product_actions ( ProductId, ActionType, ActionTime, ActionComments, AdditionalId) Values (productId, actionType,actiontime, actioncomments, additonalId) ;
		select max(Id) from product_actions into idval;

        SELECT LENGTH(operators) - LENGTH(REPLACE(operators, ';', '')) + 1 into commaCount;

        if commaCount = 0 then
            Insert Into action_operators( ActionId, Operator) Values (idval ,  operators);
        else
            SET i = 1;
    		WHILE i <= commaCount  DO
                SELECT substring_index(substring_index(operators,';', i), ';', -1) into operatorValue ;
                Insert Into action_operators( ActionId, Operator) Values (idval ,  operatorValue);
                Set i= i+1;
            END WHILE;
        end if;

    end if;

END;

//
DELIMITER ;

-- ----------------------------
-- Procedure structure for `proc_assign_machine`
-- ----------------------------
DROP PROCEDURE IF EXISTS `proc_assign_machine`;
DELIMITER //
CREATE DEFINER=`root`@`localhost` PROCEDURE `proc_assign_machine`(IN machine_number varchar(16),IN machine_name varchar(100), IN assignType int, IN assigntime datetime, IN assigncomments varchar(512), IN operators varchar(512))
BEGIN
    Declare idval int;
    Declare commaCount int;
    Declare i int;
    Declare operatorValue varchar(16);
    Declare machineId int; 
    Declare errormsg varchar(512);
    
    set errormsg ='';
    
    if machine_number  is null then 
	    set errormsg =Concat(errormsg, 'Invalid parameter: machine_number. '); 
    end if;
  
    if assignType is null then 
	    set errormsg =Concat(errormsg, 'Invalid parameter: assignType. ') ;
    end if;

    if assigntime is null then 
		set errormsg =Concat(errormsg, 'Invalid parameter: assigntime. ');
    end if;

    if operators is null then 
		set errormsg =Concat(errormsg,'Invalid parameter: operators. ');
    end if;

    if errormsg='' then 
        SELECT Id FROM machines WHERE Encode=machine_number into machineId;

        if machineId is null or machineId<=0 then
			INSERT INTO machines (Name, Encode) Values (machine_name, machine_number);
            select max(Id) from machines into machineId ;
        else
		    if machine_name is not null and machine_name <>'' then
				Update machines set Name = machine_name where Id=machineId; 
			end if;
		end if;

		Insert Into  machine_assigns ( MachineId, AssignType, AssignTime, AssignComments) Values (machineId, assignType,assigntime, assigncomments) ;
		select max(Id) from machine_assigns into idval;

		SELECT LENGTH(operators) - LENGTH(REPLACE(operators, ';', '')) + 1 into commaCount;

		if commaCount = 0 then	
			Insert Into machine_users( AssignId, UserEncode) Values (idval ,  operators);
		else
			SET i = 1;
   			WHILE i <= commaCount  DO
				SELECT substring_index(substring_index(operators,';', i), ';', -1) into operatorValue ;
				Insert Into machine_users( AssignId, UserEncode) Values (idval ,  operatorValue);
				Set i= i+1;
			END WHILE;
		end if;
	end if;
    

END;

//
DELIMITER ;