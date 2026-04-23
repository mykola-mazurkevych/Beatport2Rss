DROP VIEW "vwSubscriptions";

CREATE VIEW "vwSubscriptions" AS
SELECT
    S."Id"           AS "Id",
    S."CreatedAt"    AS "CreatedAt",
    S."Name"         AS "Name",
    S."Slug"         AS "Slug",
    S."BeatportType" AS "BeatportType",
    S."BeatportId"   AS "BeatportId",
    S."BeatportSlug" AS "BeatportSlug",
    S."ImageUri"     AS "ImageUri",
    S."RefreshedAt"  AS "RefreshedAt",
    COALESCE(
        JSONB_AGG(
            JSONB_BUILD_OBJECT(
                'UserId', T."UserId",
                'Name', T."Name",
                'Slug', T."Slug"
            )
        ) FILTER (WHERE T."Id" IS NOT NULL),
        '[]'::jsonb
    )::text AS "Tags"
FROM "Subscriptions" AS S
    LEFT JOIN "SubscriptionTags" AS ST ON ST."SubscriptionId" = S."Id"
    LEFT JOIN "Tags" AS T ON T."Id" = ST."TagId"
GROUP BY
    S."Id",
    S."CreatedAt",
    S."Name",
    S."Slug",
    S."BeatportType",
    S."BeatportId",
    S."BeatportSlug",
    S."ImageUri",
    S."RefreshedAt";