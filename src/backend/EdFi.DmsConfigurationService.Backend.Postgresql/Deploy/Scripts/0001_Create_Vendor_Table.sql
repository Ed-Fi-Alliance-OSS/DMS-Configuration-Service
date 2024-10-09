-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

CREATE TABLE cms.Vendor (
    Id BIGINT GENERATED ALWAYS AS IDENTITY (START WITH 1 INCREMENT BY 1) PRIMARY KEY,
    Company VARCHAR(256) NOT NULL,
    NamespacePrefixes VARCHAR(256),
    ContactName VARCHAR(128),
    ContactEmailAddress VARCHAR(320)
);

COMMENT ON COLUMN cms.Vendor.Id IS 'Vendor or company id';
COMMENT ON COLUMN cms.Vendor.Company IS 'Vendor or company name';
COMMENT ON COLUMN cms.Vendor.NamespacePrefixes IS 'Namespace prefix for the vendor. Multiple namespace prefixes can be provided as comma separated list if required';
COMMENT ON COLUMN cms.Vendor.ContactName IS 'Vendor contact name';
COMMENT ON COLUMN cms.Vendor.ContactEmailAddress IS 'Vendor contact email id';
