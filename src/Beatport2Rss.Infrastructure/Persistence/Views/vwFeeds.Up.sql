CREATE OR REPLACE VIEW "vwFeeds" AS
SELECT
    F."Id"        AS "Id",
    F."CreatedAt" AS "CreatedAt",
    F."UserId"    AS "UserId",
    F."Name"      AS "Name",
    F."Slug"      AS "Slug",
    F."Status" = 'Active'                AS "IsActive",
    COALESCE(FS."SubscriptionsCount", 0) AS "SubscriptionsCount"
FROM "Feeds" AS F
    LEFT JOIN
    (
        SELECT
            FS."FeedId"             AS "FeedId",
            COUNT(*)::integer       AS "SubscriptionsCount"
        FROM "FeedSubscriptions" AS FS
        GROUP BY FS."FeedId"
    ) AS FS
        ON FS."FeedId" = F."Id";