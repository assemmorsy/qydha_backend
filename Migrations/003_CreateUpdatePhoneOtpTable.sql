CREATE TABLE update_phone_requests (
    id uuid DEFAULT uuid_generate_v4() PRIMARY KEY NOT NULL,
    phone VARCHAR(30)  NOT NULL,
    otp VARCHAR(6) NOT NULL, 
    created_on TIMESTAMP NOT NULL,
    user_id uuid NOT NULL
);