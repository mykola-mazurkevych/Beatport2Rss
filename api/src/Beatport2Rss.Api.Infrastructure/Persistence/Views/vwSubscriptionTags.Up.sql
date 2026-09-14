CREATE VIEW "api"."vwSubscriptionTags" AS
SELECT
    ST."SubscriptionId" AS "SubscriptionId",
    ST."TagId"          AS "TagId",
    T."UserId"          AS "UserId",
    T."Name"            AS "Name",
    T."Slug"            AS "Slug"
FROM "api"."SubscriptionTags" AS ST
    INNER JOIN "api"."Tags" AS T ON T."Id" = ST."TagId"