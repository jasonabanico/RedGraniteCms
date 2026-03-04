/* tslint:disable */
/* eslint-disable */
// @generated
// This file was automatically generated and should not be edited.

// ====================================================
// GraphQL query operation: GetItem
// ====================================================

export interface GetItem_GetItem {
  __typename: "Item";
  id: string;
  title: string;
  slug: string | null;
  summary: string | null;
  content: string | null;
}

export interface GetItem {
  GetItem: GetItem_GetItem;
}
