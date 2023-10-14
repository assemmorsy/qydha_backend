CREATE OR REPLACE FUNCTION calc_expire_Data_and_free_Subscription_after_insert()
  RETURNS TRIGGER 
  LANGUAGE PLPGSQL
  AS
$$
declare 
	current_purchase purchases%rowtype;
	calc_expire_date TimeStamp := null;
	free_used_count int := 0;
begin 
	for current_purchase in 
		select * from purchases 
		where type = 'purchase' and user_id = NEW.user_id
		order by purchase_date
		loop 
			if current_purchase.productsku = 'free_30' then 
					free_used_count = free_used_count + 1;
			end if ; 
			
			if calc_expire_date is null or current_purchase.purchase_date > calc_expire_date then 
				calc_expire_date  = current_purchase.purchase_date + (current_purchase.number_of_days || ' days') ::interval;
			else 
				calc_expire_date  = calc_expire_date + (current_purchase.number_of_days || ' days') ::interval;
			end if; 
			
		end loop ; 
		
	update users 
	set expire_date = calc_expire_date,
		free_subscription_used = free_used_count 
	where users.id = NEW.user_id ;

    RETURN NEW;

end;
$$;


CREATE TRIGGER Insert_into_purchases
  After INSERT
  ON purchases
  FOR EACH ROW
  EXECUTE PROCEDURE calc_expire_Data_and_free_Subscription_after_insert();