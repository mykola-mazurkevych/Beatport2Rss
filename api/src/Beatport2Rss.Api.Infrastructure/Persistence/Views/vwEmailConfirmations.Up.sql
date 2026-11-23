CREATE VIEW "api"."vwEmailConfirmations" AS
SELECT
    C."Id",
    C."CreatedAt",
    C."UserId",
    C."EmailAddress",
    C."ExpiresAt",
    C."ConfirmedAt"
FROM "api"."EmailConfirmations" AS C
    INNER JOIN "api"."Users" AS U
        ON C."UserId" = U."Id"
WHERE C."EmailAddress" COLLATE "C" = U."EmailAddress" COLLATE "C";
