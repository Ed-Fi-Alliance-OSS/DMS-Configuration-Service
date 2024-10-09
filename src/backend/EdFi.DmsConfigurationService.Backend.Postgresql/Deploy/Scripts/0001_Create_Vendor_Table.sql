-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

CREATE TABLE config.Vendor (
    Id BIGINT GENERATED ALWAYS AS IDENTITY (START WITH 1 INCREMENT BY 1) PRIMARY KEY,
    Company VARCHAR(256) NOT NULL,
    NamespacePrefixes JSONB NOT NULL,
    ContactName VARCHAR(128),
    ContactEmailAddress VARCHAR(320)
);

COMMENT ON COLUMN config.Vendor.Id IS 'Vendor or company id';
COMMENT ON COLUMN config.Vendor.Company IS 'Vendor or company name';
COMMENT ON COLUMN config.Vendor.NamespacePrefixes IS 'A JSONB array of namespace prefixes associated with the vendor';
COMMENT ON COLUMN config.Vendor.ContactName IS 'Vendor contact name';
COMMENT ON COLUMN config.Vendor.ContactEmailAddress IS 'Vendor contact email id';
