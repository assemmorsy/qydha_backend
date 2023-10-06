CREATE TABLE Registration_OTP_Request (
    id uuid DEFAULT uuid_generate_v4() PRIMARY KEY NOT NULL,
    username VARCHAR(100) NOT NULL,
    password_hash VARCHAR NOT NULL,
    phone VARCHAR(30)  NOT NULL,
    otp VARCHAR(6) NOT NULL, 
    created_on TIMESTAMP NOT NULL,
    user_id uuid NULL
);