Create database RhondaLibraryPoc;

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

Create schema Persona;
Create schema Shelve;
Create schema Account;

-- Create table for books
CREATE TABLE Shelve.books (
    isbn VARCHAR(13) PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    author VARCHAR(255) NOT NULL,
    publisher VARCHAR(255) NOT NULL,
    genre VARCHAR(100) NOT NULL,
    availability BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Create table for users
CREATE TABLE Persona.users (
    user_id UUID PRIMARY KEY Default uuid_generate_v4(),
    username VARCHAR(255) NOT NULL,
    full_name VARCHAR(255) NOT NULL,
    email VARCHAR(100) NOT NULL,
    address VARCHAR(150) default NULL,
    registration_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Create table for checkouts
CREATE TABLE Account.checkouts (
    checkout_id uuid PRIMARY KEY Default uuid_generate_v1(),
    user_id uuid REFERENCES Persona.users(user_id),
    BookCheckout jsonb[] not null,
    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);
--------------------------Procedures-------------------------------------------------------------------
-- Function to add a new book
Select * from Shelve.IfBookExists('1425366598');

CREATE OR REPLACE FUNCTION Shelve.IfBookExists(vIsbn Text)
RETURNS TABLE("StatusCode" integer, "Status" text, "Message" text, "Details" text)
AS $$
BEGIN
    IF(EXISTS(SELECT isbn FROM Shelve.books where isbn = vIsbn))
    THEN
        RETURN QUERY
            SELECT 409,'BookFound', Concat('Book with ISBN -', vIsbn, ' already exists'),
                   (Select jsonb_build_object
                ('ISBN', isbn, 'Title', title, 'Author', author, 'Publisher', publisher, 'Genre', genre, 'Availability', availability, 'CreatedOn', created_at, 'LastUpdatedOn', updated_at)
                FROM Shelve.books Where isbn = vIsbn)::Text;
    ELSE
        RETURN QUERY SELECT 404,
        'BookNotFound',
        Concat('No book found with ISBN - ',vIsbn),
        (Select jsonb_build_object
                ('ISBN', vIsbn, 'Title', '', 'Author', '', 'Publisher', '', 'Genre', '', 'Availability', '', 'CreatedOn', '', 'LastUpdatedOn', ''))::Text;
    END IF;

    EXCEPTION
        WHEN SQLSTATE 'P0000' THEN
            Return QUERY SELECT 500::INTEGER, 'QueryException', 'Exception occurred whiles executing query.'::text,'Exception occurred whiles executing query.'::TEXT;
        WHEN SQLSTATE 'P0001' THEN
            Return QUERY SELECT 500::INTEGER, 'FunctionException', 'Exception occurred whiles executing function.'::text,'Exception occurred whiles executing function.'::TEXT;
        WHEN SQLSTATE '42883' THEN
            Return QUERY SELECT 500::INTEGER, 'UndefinedException', 'Undefined function exception occurred whiles executing query.'::text, 'Undefined function exception occurred whiles executing query.';
        WHEN SQLSTATE '23505' THEN
            Return QUERY SELECT 409::INTEGER, 'ISBNViolation', 'Unique violation exception occurred whiles saving record.'::text, Concat('Book with ISBN - ', vIsbn, ' already exists');
        WHEN SQLSTATE '23503' THEN
            Return QUERY SELECT 409::INTEGER, 'ReferenceViolation', 'Foreign key violation exception occurred whiles executing query.'::text, 'Foreign key violation exception occurred whiles executing query.'::text;
        WHEN SQLSTATE '08000' THEN
            Return QUERY SELECT 502::INTEGER, 'BadConnection', 'Connection exception occurred whiles executing query.'::text, 'Connection exception occurred whiles executing query.'::text;
        WHEN SQLSTATE '42703' THEN
            Return QUERY SELECT 500::INTEGER, 'UndefinedProperty', 'Undefined object property.'::text, 'Undefined table property..'::text;
end;
$$ LANGUAGE plpgsql;
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION Persona.IfUserExists(vUsername TEXT)
RETURNS TABLE("StatusCode" INTEGER, "Status" TEXT, "Message" TEXT, "Details" TEXT)
AS $$
BEGIN
    IF EXISTS (SELECT user_id FROM Persona.users WHERE user_id::TEXT = vUsername OR username = vUsername OR email = vUsername) THEN
        RETURN QUERY
            SELECT 409,
                   'UserFound',
                   CONCAT('User - ', vUsername, ' already exists'),
                   (
                       SELECT jsonb_build_object(
                                  'Id', user_id,
                                  'Username', username,
                                  'FullName', full_name,
                                  'Email', email,
                                  'Address', address,
                                  'RegistrationDate', registration_date,
                                  'CreatedOn', created_at,
                                  'LastUpdatedOn', updated_at
                              )
                       FROM Persona.users u
                       WHERE user_id::TEXT = vUsername OR username = vUsername OR email = vUsername
                   )::TEXT;
    ELSE
        RETURN QUERY
            SELECT 404,
                   'UserNotFound',
                   CONCAT('No user found with detail - ', vUsername),
                   (
                       SELECT jsonb_build_object(
                                  'Id', '',
                                  'Username', '',
                                  'FullName', '',
                                  'Email', '',
                                  'Address', '',
                                  'RegistrationDate', '',
                                  'CreatedOn', '',
                                  'LastUpdatedOn', ''
                              )
                   )::TEXT;
    END IF;

EXCEPTION
    WHEN SQLSTATE 'P0000' THEN
        RETURN QUERY SELECT 500::INTEGER, 'QueryException', 'Exception occurred while executing query.'::TEXT, 'Exception occurred while executing query.'::TEXT;
    WHEN SQLSTATE 'P0001' THEN
        RETURN QUERY SELECT 500::INTEGER, 'FunctionException', 'Exception occurred while executing function.'::TEXT, 'Exception occurred while executing function.'::TEXT;
    WHEN SQLSTATE '42883' THEN
        RETURN QUERY SELECT 500::INTEGER, 'UndefinedException', 'Undefined function exception occurred while executing query.'::TEXT, 'Undefined function exception occurred while executing query.';
    WHEN SQLSTATE '23505' THEN
        RETURN QUERY SELECT 409::INTEGER, 'ISBNViolation', 'Unique violation exception occurred while saving record.'::TEXT, CONCAT('User with ID - ', vUsername, ' already exists');
    WHEN SQLSTATE '23503' THEN
        RETURN QUERY SELECT 409::INTEGER, 'ReferenceViolation', 'Foreign key violation exception occurred while executing query.'::TEXT, 'Foreign key violation exception occurred while executing query.'::TEXT;
    WHEN SQLSTATE '08000' THEN
        RETURN QUERY SELECT 502::INTEGER, 'BadConnection', 'Connection exception occurred while executing query.'::TEXT, 'Connection exception occurred while executing query.'::TEXT;
    WHEN SQLSTATE '42703' THEN
        RETURN QUERY SELECT 500::INTEGER, 'UndefinedProperty', 'Undefined object property.'::TEXT, 'Undefined table property..'::TEXT;
END;
$$ LANGUAGE plpgsql;

------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
Select * from Shelve.add_book( '78458888', 'Art Of War',  'Moi Ton', 'hungerGames Ltd', 'Fiction', true);

CREATE OR REPLACE FUNCTION Shelve.add_book(vIsbn Text, vTitle Text, vAuthor Text, vPublisher Text, vGenre Text, vAvailability BOOLEAN)
RETURNS TABLE("StatusCode" integer, "Status" text, "Message" text, "Details" text)
AS $$
DECLARE
    NewID Varchar(13);
BEGIN
    IF(vIsbn != '' AND vTitle != '' AND vAuthor != '')
    THEN
        INSERT INTO Shelve.books(isbn, title, author, publisher, genre, availability)
        VALUES (vIsbn, vTitle, vAuthor, vPublisher, vGenre, vAvailability) RETURNING ISBN INTO NewID;

        IF(NewId IS NOT NULL OR NewID != '')
        THEN
            RETURN QUERY SELECT 200, 'SAVED', Concat('Book with ISBN - ', vIsbn, ' saved.'),
                                (Select jsonb_build_object
                ('ISBN', isbn, 'Title', title, 'Author', author, 'Publisher', publisher, 'Genre', genre, 'Availability', availability, 'CreatedOn', created_at, 'LastUpdatedOn', updated_at)
                FROM Shelve.books Where isbn = vIsbn)::Text;
        ELSE
            RETURN QUERY SELECT 400, 'FAILED', Concat('Book with ISBN - ', vIsbn, ' could not be saved.'),(Select jsonb_build_object
                ('ISBN', vIsbn, 'Title', vtitle, 'Author', vAuthor, 'Publisher', vPublisher, 'Genre', vGenre, 'Availability', vAvailability, 'CreatedOn', null, 'LastUpdatedOn', null))::Text;
        end if;
    ELSE
        RETURN QUERY SELECT 406, 'FAILED', 'Missing Book details', (Select jsonb_build_object
                ('ISBN', vIsbn, 'Title', vTitle, 'Author', vAuthor, 'Publisher', vPublisher, 'Genre', vGenre, 'Availability', vAvailability, 'CreatedOn', null, 'LastUpdatedOn', null))::Text;
    end if;

    EXCEPTION
        WHEN SQLSTATE 'P0000' THEN
            Return QUERY SELECT 500::INTEGER, 'QueryException', 'Exception occurred whiles executing query.'::text,'Exception occurred whiles executing query.'::TEXT;
        WHEN SQLSTATE 'P0001' THEN
            Return QUERY SELECT 500::INTEGER, 'FunctionException', 'Exception occurred whiles executing function.'::text,'Exception occurred whiles executing function.'::TEXT;
        WHEN SQLSTATE '42883' THEN
            Return QUERY SELECT 500::INTEGER, 'UndefinedException', 'Undefined function exception occurred whiles executing query.'::text, 'Undefined function exception occurred whiles executing query.';
        WHEN SQLSTATE '23505' THEN
            Return QUERY SELECT 409::INTEGER, 'ISBNViolation', 'Unique violation exception occurred whiles saving record.'::text, Concat('Book with ISBN - ', vIsbn, ' already exists');
        WHEN SQLSTATE '23503' THEN
            Return QUERY SELECT 409::INTEGER, 'ReferenceViolation', 'Foreign key violation exception occurred whiles executing query.'::text, 'Foreign key violation exception occurred whiles executing query.'::text;
        WHEN SQLSTATE '08000' THEN
            Return QUERY SELECT 502::INTEGER, 'BadConnection', 'Connection exception occurred whiles executing query.'::text, 'Connection exception occurred whiles executing query.'::text;
END;
$$ LANGUAGE plpgsql;

-- Function to list all books with filters
Select * from shelve.list_books();

CREATE OR REPLACE FUNCTION Shelve.list_books()
RETURNS TABLE("StatusCode" integer, "Status" text, "Message" text, "Details" text)
AS $$
BEGIN
    RETURN QUERY SELECT
        200,
        'Successful',
        'List of Books',
        (
            SELECT
                jsonb_agg(
                    jsonb_build_object (
                        'ISBN', isbn,
                        'Title', title,
                        'Author', author,
                        'Publisher', publisher,
                        'Genre', genre,
                        'Availability', availability,
                        'CreatedOn', created_at,
                        'LastUpdatedOn', updated_at
                    )
                )
            FROM
                Shelve.books
        )::text;

    EXCEPTION
        WHEN SQLSTATE 'P0000' THEN
            Return QUERY SELECT 500::INTEGER, 'QueryException', 'Exception occurred whiles executing query.'::text,'Exception occurred whiles executing query.'::TEXT;
        WHEN SQLSTATE 'P0001' THEN
            Return QUERY SELECT 500::INTEGER, 'FunctionException', 'Exception occurred whiles executing function.'::text,'Exception occurred whiles executing function.'::TEXT;
        WHEN SQLSTATE '42883' THEN
            Return QUERY SELECT 500::INTEGER, 'UndefinedException', 'Undefined function exception occurred whiles executing query.'::text, 'Undefined function exception occurred whiles executing query.';
        WHEN SQLSTATE '08000' THEN
            Return QUERY SELECT 502::INTEGER, 'BadConnection', 'Connection exception occurred whiles executing query.'::text, 'Connection exception occurred whiles executing query.'::text;
        WHEN SQLSTATE '23505' THEN
            Return QUERY SELECT 409::INTEGER, 'ISBNViolation', 'Unique violation exception occurred whiles saving record.'::text, 'Unique violation exception occurred whiles saving record.';
        WHEN SQLSTATE '23503' THEN
            Return QUERY SELECT 409::INTEGER, 'ReferenceViolation', 'Foreign key violation exception occurred whiles executing query.'::text, 'Foreign key violation exception occurred whiles executing query.'::text;
        WHEN SQLSTATE '08000' THEN
            Return QUERY SELECT 502::INTEGER, 'BadConnection', 'Connection exception occurred whiles executing query.'::text, 'Connection exception occurred whiles executing query.'::text;
END;
$$ LANGUAGE plpgsql;


-- Function to retrieve details of a specific book
Select * from shelve.get_book('78452369');

CREATE OR REPLACE FUNCTION Shelve.get_book(vIsbn Text)
RETURNS TABLE("StatusCode" integer, "Status" text, "Message" text, "Details" text)
AS $$
BEGIN
    RETURN QUERY
    SELECT 200, 'Successful', Concat('Details of ISBN - ', vIsbn, ' attached'),
           (Select jsonb_build_object
                ('ISBN', isbn, 'Title', title, 'Author', author, 'Availability', availability, 'RecordCreatedOn', created_at, 'LastUpdatedOn', updated_at)
                FROM Shelve.books Where isbn = vIsbn)::Text;

    EXCEPTION
        WHEN SQLSTATE 'P0000' THEN
            Return QUERY SELECT 500::INTEGER, 'QueryException', 'Exception occurred whiles executing query.'::text,'Exception occurred whiles executing query.'::TEXT;
        WHEN SQLSTATE 'P0001' THEN
            Return QUERY SELECT 500::INTEGER, 'FunctionException', 'Exception occurred whiles executing function.'::text,'Exception occurred whiles executing function.'::TEXT;
        WHEN SQLSTATE '42883' THEN
            Return QUERY SELECT 500::INTEGER, 'UndefinedException', 'Undefined function exception occurred whiles executing query.'::text, 'Undefined function exception occurred whiles executing query.';
        WHEN SQLSTATE '23505' THEN
            Return QUERY SELECT 409::INTEGER, 'ISBNViolation', 'Unique violation exception occurred whiles saving record.'::text, Concat('Book with ISBN - ', vIsbn, ' already exists');
        WHEN SQLSTATE '23503' THEN
            Return QUERY SELECT 409::INTEGER, 'ReferenceViolation', 'Foreign key violation exception occurred whiles executing query.'::text, 'Foreign key violation exception occurred whiles executing query.'::text;
        WHEN SQLSTATE '25P02' THEN
            Return QUERY SELECT 400::INTEGER, 'FailedSqlTransaction', 'DB Query failed to execute','DB query failed to execute';
        WHEN SQLSTATE '08000' THEN
            Return QUERY SELECT 502::INTEGER, 'BadConnection', 'Connection exception occurred whiles executing query.'::text, 'Connection exception occurred whiles executing query.'::text;
END;
$$ LANGUAGE plpgsql;

-- Function to update details of a book
CREATE OR REPLACE FUNCTION Shelve.update_book(vIsbn Text, vTitle Text, vAuthor Text, vPublisher Text, vGenre Text, vAvailability BOOLEAN)
RETURNS TABLE("StatusCode" integer, "Status" text, "Message" text, "Details" text)
AS $$
DECLARE AFFECTED Text;
BEGIN
    UPDATE Shelve.books
    SET title = vTitle, author = vAuthor, publisher = vPublisher, genre = vGenre, availability = vAvailability, updated_at = current_timestamp
    WHERE isbn = vIsbn
    Returning isbn INTO AFFECTED;

    IF(AFFECTED != '' OR AFFECTED is not NULL)
    THEN
        RETURN QUERY
        SELECT 200, 'Successful', Concat('Updated ISBN - ', vIsbn, ' with details as'),
               (Select jsonb_build_object
                ('ISBN', isbn, 'Title', title, 'Author', author, 'Availability', availability, 'RecordCreatedOn', created_at, 'LastUpdatedOn', updated_at)
                FROM Shelve.books Where isbn = vIsbn)::Text;
    ELSE
        RETURN QUERY
        SELECT 400, 'Failed', Concat('Failed to updated ISBN - ', vIsbn, ' with details. ISBN does not exists.'),
               (Select jsonb_build_object
                    ('ISBN', vIsbn, 'Title', vTitle, 'Author', vAuthor, 'Availability', false, 'RecordCreatedOn', '', 'RecordLastUpdatedOn', ''))::Text;
    end if;

    EXCEPTION
        WHEN SQLSTATE 'P0000' THEN
            Return QUERY SELECT 500::INTEGER, 'QueryException', 'Exception occurred whiles executing query.'::text,'Exception occurred whiles executing query.'::TEXT;
        WHEN SQLSTATE 'P0001' THEN
            Return QUERY SELECT 500::INTEGER, 'FunctionException', 'Exception occurred whiles executing function.'::text,'Exception occurred whiles executing function.'::TEXT;
        WHEN SQLSTATE '42883' THEN
            Return QUERY SELECT 500::INTEGER, 'UndefinedException', 'Undefined function exception occurred whiles executing query.'::text, 'Undefined function exception occurred whiles executing query.';
        WHEN SQLSTATE '23505' THEN
            Return QUERY SELECT 409::INTEGER, 'ISBNViolation', 'Unique violation exception occurred whiles saving record.'::text, Concat('Book with ISBN - ', vIsbn, ' already exists');
        WHEN SQLSTATE '23503' THEN
            Return QUERY SELECT 409::INTEGER, 'ReferenceViolation', 'Foreign key violation exception occurred whiles executing query.'::text, 'Foreign key violation exception occurred whiles executing query.'::text;
        WHEN SQLSTATE '25P02' THEN
            Return QUERY SELECT 400::INTEGER, 'FailedSqlTransaction', 'DB Query failed to execute','DB query failed to execute';
        WHEN SQLSTATE '08000' THEN
            Return QUERY SELECT 502::INTEGER, 'BadConnection', 'Connection exception occurred whiles executing query.'::text, 'Connection exception occurred whiles executing query.'::text;
END;
$$ LANGUAGE plpgsql;

-- Function to remove a book from the library
select * from Shelve.remove_book('134255565888');

CREATE OR REPLACE FUNCTION Shelve.remove_book(vIsbn Text)
RETURNS TABLE("StatusCode" integer, "Status" text, "Message" text, "Details" text)
AS $$
DECLARE DelIsbn Text; DelTitle Text; DelAuthor Text; DelCreated Text;
BEGIN
    DELETE
    FROM Shelve.books
    WHERE isbn = vIsbn
    Returning isbn, title, author,created_at into DelIsbn, DelTitle, DelAuthor, DelCreated;

    IF(DelIsbn != '' OR DelIsbn is not NULL)
    THEN
        RETURN QUERY
        SELECT 200, 'Successful', Concat('Deleted Book with ISBN - ', vIsbn),
               (Select jsonb_build_object
                ('ISBN', DelIsbn, 'Title', DelTitle, 'Author', DelAuthor, 'Availability', false, 'RecordCreatedOn', DelCreated, 'LastUpdatedOn', current_timestamp)
                )::Text;
    ELSE
        RETURN QUERY
        SELECT 400, 'Failed', Concat('Failed to delete Book with ISBN - ', vIsbn, '. ISBN does not exists.'),
               (Select jsonb_build_object
                    ('ISBN', vIsbn, 'Title', '', 'Author', '', 'Availability', false, 'RecordCreatedOn', '', 'LastUpdatedOn', ''))::Text;
    end if;

    EXCEPTION
        WHEN SQLSTATE 'P0000' THEN
            Return QUERY SELECT 500::INTEGER, 'QueryException', 'Exception occurred whiles executing query.'::text,'Exception occurred whiles executing query.'::TEXT;
        WHEN SQLSTATE 'P0001' THEN
            Return QUERY SELECT 500::INTEGER, 'FunctionException', 'Exception occurred whiles executing function.'::text,'Exception occurred whiles executing function.'::TEXT;
        WHEN SQLSTATE '42883' THEN
            Return QUERY SELECT 500::INTEGER, 'UndefinedException', 'Undefined function exception occurred whiles executing query.'::text, 'Undefined function exception occurred whiles executing query.';
        WHEN SQLSTATE '23505' THEN
            Return QUERY SELECT 409::INTEGER, 'ISBNViolation', 'Unique violation exception occurred whiles saving record.'::text, Concat('Book with ISBN - ', vIsbn, ' already exists');
        WHEN SQLSTATE '23503' THEN
            Return QUERY SELECT 409::INTEGER, 'ReferenceViolation', 'Foreign key violation exception occurred whiles executing query.'::text, 'Foreign key violation exception occurred whiles executing query.'::text;
        WHEN SQLSTATE '25P02' THEN
            Return QUERY SELECT 400::INTEGER, 'FailedSqlTransaction', 'DB Query failed to execute','DB query failed to execute';
        WHEN SQLSTATE '08000' THEN
            Return QUERY SELECT 502::INTEGER, 'BadConnection', 'Connection exception occurred whiles executing query.'::text, 'Connection exception occurred whiles executing query.'::text;
END;
$$ LANGUAGE plpgsql;

-- Function to register a new library user
Select * from Persona.users;

CREATE OR REPLACE FUNCTION Persona.register_user(vUsername Text, vFullName Text, vEmail Text, vAddress Text, vRegisterDate timestamp with time zone)
RETURNS TABLE("StatusCode" integer, "Status" text, "Message" text, "Details" text)
AS $$
DECLARE ID uuid;
BEGIN
    IF(vUsername != '' AND vFullName != '')
    THEN
        INSERT INTO Persona.users (username, full_name, email, address, registration_date, created_at, updated_at)
        VALUES (vUsername, vFullName, vEmail, vAddress, vRegisterDate, current_timestamp, current_timestamp)
        RETURNING user_id INTO ID;

        IF(ID::TEXT IS NOT NULL OR ID::TEXT != '')
        THEN
            RETURN QUERY SELECT 200, 'Successful', 'User created',(Select jsonb_build_object
                ('UserID', user_id, 'Username', username, 'FullName', full_name, 'Email', email, 'Address', address, 'RegisteredOn', registration_date, 'CreatedOn', created_at, 'LastUpdatedOn', updated_at)
                 From Persona.users Where user_id = ID)::Text;
        ELSE
            RETURN QUERY SELECT 400, 'Failed', 'User could not be created',(Select jsonb_build_object
                ('UserID', '', 'Username', vUsername, 'FullName', vFullName, 'Email', vEmail, 'Address', vAddress, 'RegisteredOn', '', 'CreatedOn', '', 'LastUpdatedOn', ''))::Text;
        end if;
    ELSE
        RETURN QUERY SELECT 406, 'FAILED', 'Missing User details', (Select jsonb_build_object
                ('UserID', '', 'Username', vUsername, 'FullName', vFullName, 'Email', vEmail, 'Address', vAddress,  'RegisteredOn', '', 'CreatedOn', '', 'LastUpdatedOn', ''))::Text;
    end if;


    EXCEPTION
        WHEN SQLSTATE 'P0000' THEN
            Return QUERY SELECT 500::INTEGER, 'QueryException', 'Exception occurred whiles executing query.'::text,'Exception occurred whiles executing query.'::TEXT;
        WHEN SQLSTATE 'P0001' THEN
            Return QUERY SELECT 500::INTEGER, 'FunctionException', 'Exception occurred whiles executing function.'::text,'Exception occurred whiles executing function.'::TEXT;
        WHEN SQLSTATE '42883' THEN
            Return QUERY SELECT 500::INTEGER, 'UndefinedException', 'Undefined function exception occurred whiles executing query.'::text, 'Undefined function exception occurred whiles executing query.';
        WHEN SQLSTATE '23505' THEN
            Return QUERY SELECT 409::INTEGER, 'ISBNViolation', 'Unique violation exception occurred whiles saving record.'::text, 'User already exists';
        WHEN SQLSTATE '23503' THEN
            Return QUERY SELECT 409::INTEGER, 'ReferenceViolation', 'Foreign key violation exception occurred whiles executing query.'::text, 'Foreign key violation exception occurred whiles executing query.'::text;
        WHEN SQLSTATE '25P02' THEN
            Return QUERY SELECT 400::INTEGER, 'FailedSqlTransaction', 'DB Query failed to execute','DB query failed to execute';
        WHEN SQLSTATE '08000' THEN
            Return QUERY SELECT 502::INTEGER, 'BadConnection', 'Connection exception occurred whiles executing query.'::text, 'Connection exception occurred whiles executing query.'::text;
END;
$$ LANGUAGE plpgsql;

-- Function to retrieve user details along with current checkouts
Select * from Persona.IfUserExists('70e2d075-5309-4d14-99af-0adb3897bcd7');
select * from Persona.Users;
Select *  from Persona.get_user_details('127dc486-7b90-420b-b0b6-8a1a5d925676');

CREATE OR REPLACE FUNCTION Persona.get_user_details(vUser_id Text)
RETURNS TABLE("StatusCode" integer, "Status" text, "Message" text, "Details" text)
AS $$
BEGIN
    IF(Exists(Select user_id from Persona.users where user_id::TEXT = vUser_id OR username = vUser_id))
    THEN
        RETURN QUERY SELECT 200, 'Successful', 'User details and books picked attached.',
        (SELECT
        jsonb_build_object(
            'UserID', u.user_id,
            'Username', u.username,
            'FullName', u.full_name,
            'Email', u.email,
            'Address', u.address,
            'RegisteredOn', u.registration_date,
            'Checkouts', COALESCE(
                (
                    SELECT jsonb_agg(
                        jsonb_build_object(
                            'CheckoutId', c.checkout_id,
                            'Books', c.bookcheckout,
                            'CheckoutDate', c.created_at
                        )
                    )
                    FROM account.checkouts c
                    JOIN shelve.books b ON c.bookcheckout->>'isbn' = b.isbn
                    WHERE c.user_id = u.user_id
                ),
                '[]'::jsonb
            )
        )
    FROM persona.users u)::TEXT;
    ELSE
        RETURN QUERY
            SELECT 404, 'NotFound', 'User details not found',
            (SELECT
                jsonb_build_object(
                    'UserID', '',
                    'Username', '',
                    'FullName', '',
                    'Email', '',
                    'Address', '',
                    'RegisteredOn', '',
                    'Checkouts', COALESCE(
                        (
                            SELECT jsonb_agg(
                                jsonb_build_object(
                                    'CheckoutId', '',
                                    'Books', '[]',
                                    'CheckoutDate', ''
                                )
                            )
                        ),
                        '[]'::jsonb
                    )
                )
            )::TEXT;
    end if;


    EXCEPTION
        WHEN SQLSTATE 'P0000' THEN
            Return QUERY SELECT 500::INTEGER, 'QueryException', 'Exception occurred whiles executing query.'::text,'Exception occurred whiles executing query.'::TEXT;
        WHEN SQLSTATE 'P0001' THEN
            Return QUERY SELECT 500::INTEGER, 'FunctionException', 'Exception occurred whiles executing function.'::text,'Exception occurred whiles executing function.'::TEXT;
        WHEN SQLSTATE '42883' THEN
            Return QUERY SELECT 500::INTEGER, 'UndefinedException', 'Undefined function exception occurred whiles executing query.'::text, 'Undefined function exception occurred whiles executing query.';
        WHEN SQLSTATE '23505' THEN
            Return QUERY SELECT 409::INTEGER, 'ISBNViolation', 'Unique violation exception occurred whiles saving record.'::text, 'User already exists';
        WHEN SQLSTATE '23503' THEN
            Return QUERY SELECT 409::INTEGER, 'ReferenceViolation', 'Foreign key violation exception occurred whiles executing query.'::text, 'Foreign key violation exception occurred whiles executing query.'::text;
        WHEN SQLSTATE '25P02' THEN
            Return QUERY SELECT 400::INTEGER, 'FailedSqlTransaction', 'DB Query failed to execute','DB query failed to execute';
        WHEN SQLSTATE '08000' THEN
            Return QUERY SELECT 502::INTEGER, 'BadConnection', 'Connection exception occurred whiles executing query.'::text, 'Connection exception occurred whiles executing query.'::text;
END;
$$ LANGUAGE plpgsql;

-- Function to check out one or more books for a user
CREATE OR REPLACE FUNCTION Account.checkout_books(vUser_id TEXT, vBooks TEXT)
RETURNS TABLE("StatusCode" INTEGER, "Status" TEXT, "Message" TEXT, "Details" TEXT)
AS
$$
DECLARE
    CHK_ID TEXT;
BEGIN
    INSERT INTO Account.checkouts (user_id, bookcheckout, created_at, updated_at)
    VALUES (vUser_id::UUID, ARRAY[CAST(vBooks AS JSONB)], current_timestamp::timestamp without time zone, current_timestamp::timestamp without time zone)
    RETURNING checkout_id::TEXT INTO CHK_ID;

    IF CHK_ID IS NOT NULL AND CHK_ID != ''
    THEN
        RETURN QUERY
        SELECT
            200,
            'SUCCESS',
            'Checkout books attached',
            jsonb_build_object(
                'User', jsonb_build_object(
                    'UserID', vUser_id,
                    'FullName', COALESCE(u.full_name, ''),
                    'Email', COALESCE(u.email, '')
                ),
                'Checkouts', jsonb_build_object(
                    'CheckoutId', CHK_ID,
                    'Books', COALESCE(
                        (SELECT jsonb_agg(jsonb_set(book_info, '{title}', to_jsonb(title)))
                         FROM (
                             SELECT jsonb_array_elements(unnest(c.bookcheckout)) AS book_info, 1 AS constant_value
                             FROM Persona.users u
                             LEFT JOIN Account.checkouts c ON u.user_id = c.user_id
                               AND u.user_id::TEXT = vUser_id
                               AND c.checkout_id::TEXT = CHK_ID
                             LEFT JOIN unnest(c.bookcheckout) bd ON true
                             WHERE u.user_id::TEXT = vUser_id
                         ) AS tb
                         LEFT JOIN Shelve.books b ON tb.book_info::jsonb ->> 'BookId' = b.isbn
                         AND tb.book_info::jsonb ->> 'Returned' = false::TEXT
                        )::JSONB, '[]'::JSONB)
                )
            )::TEXT
            FROM persona.users u
            LEFT JOIN Account.checkouts c ON u.user_id = c.user_id
            WHERE c.checkout_id::TEXT = CHK_ID AND u.user_id::TEXT = vUser_id;
    ELSE
        RETURN QUERY
        SELECT
            400,
            'FAILED',
            'Checkout books not found',
            jsonb_build_object(
                'User', json_build_object(
                    'UserID', '',
                    'FullName', '',
                    'Email', ''
                ),
                'Checkouts', json_build_object(
                    'CheckoutId', '',
                    'Books', '[]'::JSONB
                )
            )::TEXT;
    END IF;

EXCEPTION
    WHEN SQLSTATE 'P0000' THEN
        RETURN QUERY SELECT 500, 'QueryException', 'Exception occurred while executing query.', 'Exception occurred while executing query.';
    WHEN OTHERS THEN
        RETURN QUERY SELECT 500, 'GeneralException', 'General exception occurred while executing query.', 'General exception occurred while executing query.';
END;
$$ LANGUAGE plpgsql;

------------------------------------------------------------------------------------------------------------------------
Select * from Account.Get_Checkout_Books('bebf1d3a-4ae4-4263-90aa-361e8f5f7ddf','21396c12-84bc-11ee-adfe-0242ac110005');

CREATE OR REPLACE FUNCTION Account.Get_Checkout_Books(vUserID TEXT, vCheckoutId TEXT)
RETURNS TABLE ("StatusCode" INTEGER, "Status" TEXT, "Message" TEXT, "Details" TEXT)
AS
$$
BEGIN
    IF EXISTS (SELECT 1 FROM Account.checkouts WHERE checkout_id::TEXT = vCheckoutId AND user_id::TEXT = vUserID)
    THEN
        RETURN QUERY
        SELECT
            200,
            'SUCCESS',
            'Checkout books attached',
            jsonb_build_object(
                'User', jsonb_build_object(
                    'UserID', vUserID,
                    'FullName', COALESCE(u.full_name, ''),
                    'Email', COALESCE(u.email, '')
                ),
                'Checkouts', jsonb_build_object(
                    'CheckoutId', vCheckoutId,
                    'Books', COALESCE(
                        (SELECT jsonb_agg(jsonb_set(book_info, '{title}', to_jsonb(title)))
                         FROM (
                             SELECT jsonb_array_elements(unnest(c.bookcheckout)) AS book_info, 1 AS constant_value
                             FROM Persona.users u
                             LEFT JOIN Account.checkouts c ON u.user_id = c.user_id
                               AND u.user_id::TEXT = vUserID
                               AND c.checkout_id::TEXT = vCheckoutId
                             LEFT JOIN unnest(c.bookcheckout) bd ON true
                             WHERE u.user_id::TEXT = vUserID
                         ) AS tb
                         LEFT JOIN Shelve.books b ON tb.book_info::jsonb ->> 'BookId' = b.isbn
                         AND tb.book_info::jsonb ->> 'Returned' = false::TEXT
                        )::JSONB, '[]'::JSONB)
                )
            )::TEXT
            FROM persona.users u
            LEFT JOIN Account.checkouts c ON u.user_id = c.user_id
            WHERE c.checkout_id::TEXT = vCheckoutId AND u.user_id::TEXT = vUserID;
    ELSE
        RETURN QUERY
        SELECT
            400,
            'FAILED',
            'Checkout books not found',
            jsonb_build_object(
                'User', json_build_object(
                    'UserID', '',
                    'FullName', '',
                    'Email', ''
                ),
                'Checkouts', json_build_object(
                    'CheckoutId', '',
                    'Books', '[]'::JSONB
                )
            )::TEXT;
    END IF;

    EXCEPTION
    WHEN SQLSTATE 'P0000' THEN
        RETURN QUERY SELECT 500, 'QueryException', 'Exception occurred while executing query.', 'Exception occurred while executing query.';
    WHEN OTHERS THEN
        RETURN QUERY SELECT 500, 'GeneralException', 'General exception occurred while executing query.', 'General exception occurred while executing query.';
END;
$$ LANGUAGE plpgsql;
-----------------------------------------------------------------------------------------------------------------------------------------
Select * from Account.Get_Checkout_BookDetail('62a16f02-84c0-11ee-a16f-0242ac110005', '9780547928210');

CREATE OR REPLACE FUNCTION Account.Get_Checkout_BookDetail(vCheckoutId TEXT, vBookId TEXT)
RETURNS TABLE ("StatusCode" INTEGER, "Status" TEXT, "Message" TEXT, "Details" TEXT)
AS
$$
BEGIN
    IF EXISTS (SELECT 1 FROM Account.checkouts WHERE checkout_id::TEXT = vCheckoutId)
    THEN
        RETURN QUERY
        SELECT
            200,
            'SUCCESS',
            'Checkout books attached',
            (Select coalesce(
                (Select jsonb_set(book_info, '{title}', to_jsonb(b.title)) from
                (
                    SELECT jsonb_array_elements(unnest(c.bookcheckout)) AS book_info
                    FROM Account.checkouts c JOIN unnest(c.bookcheckout) bd ON true
                    WHERE checkout_id = vCheckoutId) as bif
                    LEFT JOIN Shelve.books b ON bif.book_info::jsonb ->> 'BookId' = b.isbn
                where bif.book_info ->> 'BookId' = vBookId),'{}')
            )::TEXT;
    ELSE
        RETURN QUERY
        SELECT
            400,
            'FAILED',
            'Checkout book not found',
            '{}'::TEXT;
    END IF;

    EXCEPTION
    WHEN SQLSTATE 'P0000' THEN
        RETURN QUERY SELECT 500, 'QueryException', 'Exception occurred while executing query.', 'Exception occurred while executing query.';
    WHEN OTHERS THEN
        RETURN QUERY SELECT 500, 'GeneralException', 'General exception occurred while executing query.', 'General exception occurred while executing query.';
END;
$$ LANGUAGE plpgsql;
------------------------------------------------------------------------------------------------------------------------------------------------
-- Call the function to update the Returned status for a specific BookId
SELECT update_book_returned_status('62a16f02-84c0-11ee-a16f-0242ac110005', '9780547928227');

CREATE OR REPLACE FUNCTION update_book_returned_status(p_checkout_id Text, p_book_id text)
RETURNS VOID AS $$
DECLARE
    updated_data jsonb;
BEGIN
    -- Update the element in the jsonb[] array
    UPDATE Account.checkouts
    SET bookcheckout = (
        SELECT array_agg(
            CASE
                WHEN element ->> 'BookId' = p_book_id THEN jsonb_set(element, '{Returned}', 'true'::jsonb)
                ELSE element
            END
        )
        FROM unnest(bookcheckout) AS element
    )
    WHERE checkout_id = p_checkout_id;

    -- Optional: Fetch the updated array for further processing or logging
    SELECT bookcheckout INTO updated_data
    FROM Account.checkouts
    WHERE checkout_id::TEXT = p_checkout_id;

    -- Optional: Print the updated array for verification
    RAISE NOTICE 'Updated BookCheckout: %', updated_data;
END;
$$ LANGUAGE PLPGSQL;
-------------------------------------------------------------------------------------------------------------------
Select * from Persona.Users;
Select * from Shelve.books;
select * from Account.checkouts;
-------------------------------------------------------------------------------------------------------------------

