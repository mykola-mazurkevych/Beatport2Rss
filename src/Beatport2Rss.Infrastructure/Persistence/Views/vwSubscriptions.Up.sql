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
    S."RefreshedAt"  AS "RefreshedAt"
FROM "Subscriptions" AS S;