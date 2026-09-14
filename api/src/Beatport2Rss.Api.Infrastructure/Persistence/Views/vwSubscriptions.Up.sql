CREATE VIEW "api"."vwSubscriptions" AS
SELECT
    S."Id"                              AS "Id",
    S."CreatedAt"                       AS "CreatedAt",
    S."Type"                            AS "Type",
    S."Name"                            AS "Name",
    S."Slug"                            AS "Slug",
    S."BeatportId"                      AS "BeatportId",
    S."BeatportSlug"                    AS "BeatportSlug",
    S."ImageUri"                        AS "ImageUri",
    C."Name"                            AS "Country",
    COALESCE(FS."SubscribersCount", 0)  AS "SubscribersCount"
FROM "api"."Subscriptions" AS S
    LEFT JOIN "api"."Countries" AS C
        ON C."Id" = S."CountryCode"
    LEFT JOIN (
        SELECT
            FS."SubscriptionId" AS "SubscriptionId",
            COUNT(*)            AS "SubscribersCount"
        FROM "api"."FeedSubscriptions" AS FS
        GROUP BY FS."SubscriptionId"
    ) AS FS
        ON FS."SubscriptionId" = S."Id";