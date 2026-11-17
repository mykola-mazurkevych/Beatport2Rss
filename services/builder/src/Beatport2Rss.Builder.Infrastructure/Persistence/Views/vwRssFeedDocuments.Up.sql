CREATE VIEW "builder"."vwRssFeedDocuments" AS
SELECT
    F."Id"          AS "Id",
    F."Slug"        AS "Slug",
    D."GeneratedAt" AS "GeneratedAt",
    D."Content"     AS "Content"
FROM "builder"."Feeds" AS F
    INNER JOIN "builder"."RssFeedDocuments" AS D
        ON D."Id" = F."Id";