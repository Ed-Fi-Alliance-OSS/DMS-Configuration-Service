-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

CREATE TABLE cms.Application (
	Id BIGINT GENERATED ALWAYS AS IDENTITY (START WITH 1 INCREMENT BY 1) PRIMARY KEY,
    ApplicationName VARCHAR(255) NOT NULL,
    VendorId BIGINT NOT NULL,
    ClaimSetName VARCHAR(255) NOT NULL,   
    CONSTRAINT fk_vendor FOREIGN KEY (vendorId) REFERENCES cms.Vendor(id) ON DELETE CASCADE
);

COMMENT ON COLUMN cms.Application.Id IS 'Application id';
COMMENT ON COLUMN cms.Application.ApplicationName IS 'Application name';
COMMENT ON COLUMN cms.Application.VendorId IS 'Vendor or company id';
COMMENT ON COLUMN cms.Application.ClaimSetName IS 'Claim set name';

CREATE TABLE cms.ApplicationProfile (
    ApplicationId BIGINT NOT NULL,
    ProfileId BIGINT NOT NULL,
    CONSTRAINT pk_applicationProfile PRIMARY KEY (ApplicationId, ProfileId),
    CONSTRAINT fk_application FOREIGN KEY (ApplicationId) REFERENCES cms.Application(id) ON DELETE CASCADE
);

COMMENT ON TABLE cms.ApplicationProfile IS 'Relationship of applications with profiles';
COMMENT ON COLUMN cms.ApplicationProfile.ApplicationId IS 'Application id';
COMMENT ON COLUMN cms.ApplicationProfile.ProfileId IS 'Profile id';

CREATE TABLE cms.ApplicationEducationOrganization (
    ApplicationId BIGINT NOT NULL,
    EducationOrganizationId BIGINT NOT NULL,
    CONSTRAINT pk_applicationEducationOrganization PRIMARY KEY (ApplicationId, EducationOrganizationId),
    CONSTRAINT fk_application_educationOrganization FOREIGN KEY (ApplicationId) REFERENCES cms.Application(id) ON DELETE CASCADE
);

COMMENT ON TABLE cms.ApplicationEducationOrganization IS 'Relationship of applications with educational organizations';
COMMENT ON COLUMN cms.ApplicationEducationOrganization.ApplicationId IS 'Application id';
COMMENT ON COLUMN cms.ApplicationEducationOrganization.EducationOrganizationId IS 'Education organization id';


-- Crear tabla para almacenar los odsInstanceIds relacionados
CREATE TABLE cms.ApplicationOdsInstance (
    ApplicationId BIGINT NOT NULL,
    OdsInstanceId BIGINT NOT NULL,
    CONSTRAINT pk_applicationOdsInstance PRIMARY KEY (ApplicationId, OdsInstanceId),
    CONSTRAINT fk_application_odsInstance FOREIGN KEY (ApplicationId) REFERENCES cms.Application(id) ON DELETE CASCADE
);

COMMENT ON TABLE cms.ApplicationOdsInstance IS 'Relationship of applications with ODS instances';
COMMENT ON COLUMN cms.ApplicationOdsInstance.ApplicationId IS 'Application id';
COMMENT ON COLUMN cms.ApplicationOdsInstance.OdsInstanceId IS 'ODS instance id';
