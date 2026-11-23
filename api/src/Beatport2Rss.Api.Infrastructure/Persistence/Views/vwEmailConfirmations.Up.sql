CREATE VIEW "api"."vwEmailConfirmations" AS
SELECT
    C."Id"            AS "Id",
    C."CreatedAt"     AS "CreatedAt",
    C."UserId"        AS "UserId",
    C."EmailAddress"  AS "EmailAddress",
    C."ExpiresAt"     AS "ExpiresAt",
    C."ConfirmedAt"   AS "ConfirmedAt"
FROM "api"."EmailConfirmations" AS C