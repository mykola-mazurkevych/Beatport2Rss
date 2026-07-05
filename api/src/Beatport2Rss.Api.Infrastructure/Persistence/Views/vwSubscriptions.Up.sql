DROP VIEW IF EXISTS "vwSubscriptions";

CREATE VIEW "vwSubscriptions" AS
SELECT
    S."Id"                             AS "Id",
    S."CreatedAt"                      AS "CreatedAt",
    S."Name"                           AS "Name",
    S."Slug"                           AS "Slug",
    S."BeatportType"                   AS "BeatportType",
    S."BeatportId"                     AS "BeatportId",
    S."BeatportSlug"                   AS "BeatportSlug",
    S."ImageUri"                       AS "ImageUri",
    C."Name"                           AS "Country",
    COALESCE(FS."SubscribersCount", 0) AS "SubscribersCount"
FROM "Subscriptions" AS S
    LEFT JOIN "Countries" AS C
        ON C."Id" = S."CountryCode"
    LEFT JOIN (
        SELECT
            FS."SubscriptionId" AS "SubscriptionId",
            COUNT(*)            AS "SubscribersCount"
        FROM "FeedSubscriptions" AS FS
        GROUP BY FS."SubscriptionId"
    ) AS FS
        ON FS."SubscriptionId" = S."Id";