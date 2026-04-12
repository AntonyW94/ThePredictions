-- Phase 0b: Update World Cup 2026 team logos to circle-flags (circular SVG)
-- Source: https://github.com/HatScripts/circle-flags (MIT licence)
-- CDN: jsDelivr (pinned to v2.8.2)
-- Run after phase-0-insert-world-cup-2026-teams.sql
--
-- Uses ISO 3166-1 alpha-2 codes. England/Scotland use GB subdivisions (gb-eng, gb-sct).

UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/be.svg' WHERE [ApiTeamId] = 1;    -- Belgium
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/fr.svg' WHERE [ApiTeamId] = 2;    -- France
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/hr.svg' WHERE [ApiTeamId] = 3;    -- Croatia
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/se.svg' WHERE [ApiTeamId] = 5;    -- Sweden
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/br.svg' WHERE [ApiTeamId] = 6;    -- Brazil
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/uy.svg' WHERE [ApiTeamId] = 7;    -- Uruguay
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/co.svg' WHERE [ApiTeamId] = 8;    -- Colombia
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/es.svg' WHERE [ApiTeamId] = 9;    -- Spain
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/gb-eng.svg' WHERE [ApiTeamId] = 10;  -- England
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/pa.svg' WHERE [ApiTeamId] = 11;   -- Panama
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/jp.svg' WHERE [ApiTeamId] = 12;   -- Japan
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/sn.svg' WHERE [ApiTeamId] = 13;   -- Senegal
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/ch.svg' WHERE [ApiTeamId] = 15;   -- Switzerland
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/mx.svg' WHERE [ApiTeamId] = 16;   -- Mexico
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/kr.svg' WHERE [ApiTeamId] = 17;   -- South Korea
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/au.svg' WHERE [ApiTeamId] = 20;   -- Australia
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/ir.svg' WHERE [ApiTeamId] = 22;   -- Iran
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/sa.svg' WHERE [ApiTeamId] = 23;   -- Saudi Arabia
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/de.svg' WHERE [ApiTeamId] = 25;   -- Germany
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/ar.svg' WHERE [ApiTeamId] = 26;   -- Argentina
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/pt.svg' WHERE [ApiTeamId] = 27;   -- Portugal
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/tn.svg' WHERE [ApiTeamId] = 28;   -- Tunisia
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/ma.svg' WHERE [ApiTeamId] = 31;   -- Morocco
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/eg.svg' WHERE [ApiTeamId] = 32;   -- Egypt
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/cz.svg' WHERE [ApiTeamId] = 770;  -- Czech Republic
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/at.svg' WHERE [ApiTeamId] = 775;  -- Austria
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/tr.svg' WHERE [ApiTeamId] = 777;  -- Turkiye
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/no.svg' WHERE [ApiTeamId] = 1090; -- Norway
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/gb-sct.svg' WHERE [ApiTeamId] = 1108; -- Scotland
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/ba.svg' WHERE [ApiTeamId] = 1113; -- Bosnia & Herzegovina
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/nl.svg' WHERE [ApiTeamId] = 1118; -- Netherlands
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/ci.svg' WHERE [ApiTeamId] = 1501; -- Ivory Coast
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/gh.svg' WHERE [ApiTeamId] = 1504; -- Ghana
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/cd.svg' WHERE [ApiTeamId] = 1508; -- Congo DR
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/za.svg' WHERE [ApiTeamId] = 1531; -- South Africa
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/dz.svg' WHERE [ApiTeamId] = 1532; -- Algeria
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/cv.svg' WHERE [ApiTeamId] = 1533; -- Cape Verde Islands
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/jo.svg' WHERE [ApiTeamId] = 1548; -- Jordan
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/iq.svg' WHERE [ApiTeamId] = 1567; -- Iraq
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/uz.svg' WHERE [ApiTeamId] = 1568; -- Uzbekistan
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/qa.svg' WHERE [ApiTeamId] = 1569; -- Qatar
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/py.svg' WHERE [ApiTeamId] = 2380; -- Paraguay
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/ec.svg' WHERE [ApiTeamId] = 2382; -- Ecuador
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/us.svg' WHERE [ApiTeamId] = 2384; -- USA
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/ht.svg' WHERE [ApiTeamId] = 2386; -- Haiti
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/nz.svg' WHERE [ApiTeamId] = 4673; -- New Zealand
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/ca.svg' WHERE [ApiTeamId] = 5529; -- Canada
UPDATE [Teams] SET [LogoUrl] = N'https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/cw.svg' WHERE [ApiTeamId] = 5530; -- Curacao

-- Verify: spot-check a few URLs
-- https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/gb-eng.svg  (England)
-- https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/gb-sct.svg  (Scotland)
-- https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/br.svg      (Brazil)
-- https://cdn.jsdelivr.net/npm/circle-flags@2.8.2/flags/cw.svg      (Curacao)
