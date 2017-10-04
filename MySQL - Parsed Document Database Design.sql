create schema if not exists ParsedDocuments default char set utf8;

Drop Table ParsedDocuments.WordMapPositions_TBL;
Drop Table ParsedDocuments.NeighborWords_TBL;
Drop Table ParsedDocuments.WordMaps_TBL;
Drop Table ParsedDocuments.Words_TBL;
Drop Table ParsedDocuments.Documents_TBL;


create table if not exists ParsedDocuments.Documents_TBL(
	uniqueIdNumeric BigInt not null  auto_increment,
    uniqueIdGuid Char(36) not null,
    hashValue Char(64) not null,
    fullDocumentText longtext not null,
    createdOn datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    createdBy datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
	PRIMARY KEY (uniqueIdNumeric),
	UNIQUE KEY Documents_UKY_uniqueIdGuid (uniqueIdGuid),
    index      Documents_IDX_uniqueIdNumeric_uniqueIdGuid (uniqueIdNumeric, uniqueIdGuid),
    index      Documents_IDX_uniqueIdNumeric_hashValue (uniqueIdNumeric, hashValue),
    index      Documents_IDX_hashValue_uniqueIdNumeric (hashValue,uniqueIdNumeric),
    index      Documents_IDX_uniqueIdGuid_hashValue (uniqueIdGuid, hashValue)
) ENGINE=INNODB;
    
create table if not exists ParsedDocuments.Words_TBL(
	uniqueIdNumeric bigint not null auto_increment,
    uniqueIdGuid Char(36) not null,
    wordText varChar(100) not null,
    createdOnTimestamp datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    updatedOnTimestamp datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
	PRIMARY KEY (uniqueIdNumeric),
	UNIQUE KEY Words_UKY_uniqueIdGuid (uniqueIdGuid),
	UNIQUE KEY Words_UKY_wordText (wordText),
    index      Words_IDX_uniqueIdNumeric_uniqueIdGuid (uniqueIdNumeric, uniqueIdGuid)
) ENGINE=INNODB;

create table if not exists ParsedDocuments.WordMaps_TBL(
	uniqueIdNumeric bigint not null auto_increment,
    uniqueIdGuid Char(36) not null,
	documentIdNumeric BigInt not null,
    wordIdNumeric BigInt not null,
    createdOnTimestamp datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    updatedOnTimestamp datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
	PRIMARY KEY (uniqueIdNumeric),
	UNIQUE KEY WordMaps_UKY_uniqueIdGuid (uniqueIdGuid),
	UNIQUE KEY WordMaps_UKY_documentIdNumeric_wordIdNumeric (documentIdNumeric, wordIdNumeric),
    CONSTRAINT WordMaps_FKY_documentIdNumeric FOREIGN KEY (documentIdNumeric) REFERENCES Documents_TBL (uniqueIdNumeric) ON DELETE RESTRICT,
    CONSTRAINT WordMaps_FKY_wordIdNumeric FOREIGN KEY (wordIdNumeric) REFERENCES Words_TBL (uniqueIdNumeric) ON DELETE RESTRICT,
    index      WordMaps_IDX_uniqueIdGuid_uniqueIdNumeric (uniqueIdGuid, uniqueIdNumeric),
    index      WordMaps_IDX_uniqueIdNumeric_uniqueIdGuid (uniqueIdNumeric, uniqueIdGuid),
    index      WordMaps_IDX_wordIdNUmeric_documentIdNumeric (wordIdNumeric, documentIdNumeric)
) ENGINE=INNODB;

create table if not exists ParsedDocuments.WordMapPositions_TBL(
	uniqueIdNumeric bigint not null auto_increment,
    wordMapIdNumeric BigInt not null,
    positionInDocument BigInt not null,
    createdOnTimestamp datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    updatedOnTimestamp datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
	PRIMARY KEY (uniqueIdNumeric),
	UNIQUE KEY WordMapPositions_UKY_wordMapIdNumeric_positionInDocument (wordMapIdNumeric, positionInDocument),
    CONSTRAINT WordMapPositions_FKY_wordMapIdNumeric FOREIGN KEY (wordMapIdNumeric) REFERENCES WordMaps_TBL (uniqueIdNumeric) ON DELETE RESTRICT,
    index      WordMapPositions_IDX_positionInDocument_wordMapIdNumeric (positionInDocument, wordMapIdNumeric)
) ENGINE=INNODB;


create table if not exists ParsedDocuments.NeighborWords_TBL(
	uniqueIdNumeric bigint not null auto_increment,
    wordMapIdNumeric BigInt not null,
    neighborWordIdNumeric BigInt Not Null,
    relativePositionInDocument SmallInt Not Null,
    createdOnTimestamp datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    updatedOnTimestamp datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
	PRIMARY KEY (uniqueIdNumeric),
	UNIQUE KEY NeighborWords_UKY_wordMapIdNumeric_neighborWordIdNumeric (wordMapIdNumeric, neighborWordIdNumeric),
    CONSTRAINT FK_NeighborWords_wordMapIdNumeric FOREIGN KEY (wordMapIdNumeric) REFERENCES WordMaps_TBL (uniqueIdNumeric) ON DELETE RESTRICT,
    CONSTRAINT FK_NeighborWords_neighborWordIdNumeric FOREIGN KEY (neighborWordIdNumeric) REFERENCES Words_TBL (uniqueIdNumeric) ON DELETE RESTRICT
) ENGINE=INNODB;