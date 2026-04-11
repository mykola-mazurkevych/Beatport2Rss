CREATE OR REPLACE VIEW "vwSessions" AS
SELECT
    S."Id"                    AS "Id",
    S."CreatedAt"             AS "CreatedAt",
    S."UserId"                AS "UserId",
    U."EmailAddress"          AS "EmailAddress",
    U."FirstName"             AS "FirstName",
    U."LastName"              AS "LastName",
    S."UserAgent"             AS "UserAgent",
    S."IpAddress"             AS "IpAddress",
    S."RefreshTokenExpiresAt" AS "RefreshTokenExpiresAt"
FROM "Sessions" AS S
    INNER JOIN "Users" AS U ON U."Id" = S."UserId";