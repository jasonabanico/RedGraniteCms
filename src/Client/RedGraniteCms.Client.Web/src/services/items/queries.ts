import gql from "graphql-tag";

export const GET_ITEMS = gql`
    query GetItems($status: ItemStatus, $visibility: ItemVisibility, $contentType: String, $isoMaxDate: String, $count: Int, $skip: Int) {
        GetItems(status: $status, visibility: $visibility, contentType: $contentType, isoMaxDate: $isoMaxDate, count: $count, skip: $skip) {
            id
            title
            slug
            summary
            status
        }
    }
`;

export const GET_ITEM = gql`
    query GetItem($id: String, $slug: String, $status: ItemStatus, $visibility: ItemVisibility) {
        GetItem(id: $id, slug: $slug, status: $status, visibility: $visibility) {
            id
            title
            slug
            summary
            content
            status
        }
    }
`;
