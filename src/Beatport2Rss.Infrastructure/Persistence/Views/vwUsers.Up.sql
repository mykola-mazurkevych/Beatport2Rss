CREATE OR REPLACE VIEW "vwUsers" AS
SELECT
    U."Id" AS "Id",
    U."CreatedAt" AS "CreatedAt",
    U."EmailAddress" AS "EmailAddress",
    U."PasswordHash" AS "PasswordHash",
    U."FirstName" AS "FirstName",
    U."LastName" AS "LastName",
    NULLIF(TRIM(CONCAT_WS(' ', U."FirstName", U."LastName")), '') AS "FullName",
    U."Status" AS "Status",
    U."Status" = 'Active' AS "IsActive",
    COALESCE(F."FeedsCount", 0) AS "FeedsCount",
    COALESCE(T."TagsCount", 0) AS "TagsCount"
FROM "Users" AS U
    LEFT JOIN
    (
        SELECT
            F."UserId" AS "UserId",
            COUNT(*)::integer AS "FeedsCount"
        FROM "Feeds" AS F
        GROUP BY F."UserId"
    ) AS F
        ON F."UserId" = U."Id"
    LEFT JOIN
    (
        SELECT
            T."UserId" AS "UserId",
            COUNT(*)::integer AS "TagsCount"
        FROM "Tags" AS T
        GROUP BY T."UserId"
    ) AS T
        ON T."UserId" = U."Id";