CREATE VIEW "api"."vwEmailConfirmations" AS
SELECT
    C."Id"          AS "Id",
    C."ExpiresAt"   AS "ExpiresAt",
    C."ConfirmedAt" AS "ConfirmedAt"
FROM "api"."EmailConfirmations" AS C;