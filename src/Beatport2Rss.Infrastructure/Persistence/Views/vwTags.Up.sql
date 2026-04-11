CREATE OR REPLACE VIEW "vwTags" AS
SELECT
    T."Id" AS "Id",
    T."CreatedAt" AS "CreatedAt",
    T."UserId" AS "UserId",
    T."Name" AS "Name",
    T."Slug" AS "Slug"
FROM "Tags" AS T;