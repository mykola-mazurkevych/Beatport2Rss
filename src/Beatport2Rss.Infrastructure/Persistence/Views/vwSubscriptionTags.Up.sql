CREATE VIEW "vwSubscriptionTags" AS
SELECT
    ST."SubscriptionId" AS "SubscriptionId",
    ST."TagId"          AS "TagId",
    T."UserId"          AS "UserId",
    T."Name"            AS "Name",
    T."Slug"            AS "Slug"
FROM "SubscriptionTags" AS ST
    INNER JOIN "Tags" AS T ON T."Id" = ST."TagId"