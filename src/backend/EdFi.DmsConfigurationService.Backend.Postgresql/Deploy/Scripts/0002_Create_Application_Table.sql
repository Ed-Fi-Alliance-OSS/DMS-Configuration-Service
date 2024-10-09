-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

CREATE TABLE config.Application (
	Id BIGINT GENERATED ALWAYS AS IDENTITY (START WITH 1 INCREMENT BY 1) PRIMARY KEY,
    ApplicationName VARCHAR(255) NOT NULL,
    VendorId BIGINT NOT NULL,
    ClaimSetName VARCHAR(255) NOT NULL,
    EducationOrganizations JSONB NOT NULL,
    CONSTRAINT fk_vendor FOREIGN KEY (VendorId) REFERENCES config.Vendor(Id) ON DELETE CASCADE
);

COMMENT ON COLUMN config.Application.Id IS 'Application id';
COMMENT ON COLUMN config.Application.ApplicationName IS 'Application name';
COMMENT ON COLUMN config.Application.VendorId IS 'Vendor or company id';
COMMENT ON COLUMN config.Application.ClaimSetName IS 'Claim set name';
COMMENT ON COLUMN config.Application.EducationOrganizations IS 'Education organization ids';
