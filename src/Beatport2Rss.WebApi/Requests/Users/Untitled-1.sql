SELECT "Id",
       "EmailAddress",
       "PasswordHash",
       "FirstName",
       "LastName",
       "Status",
       "CreatedAt"
FROM public."Users"
LIMIT 1000;