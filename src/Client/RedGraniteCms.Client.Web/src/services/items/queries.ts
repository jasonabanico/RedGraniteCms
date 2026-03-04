import gql from "graphql-tag";

export const GET_ITEMS = gql`
    query GetItems($isoMaxDate: String, $count: Int) {
        GetItems(isoMaxDate: $isoMaxDate, count: $count) {
            id,
            title,
            slug,
            summary
        }
    }
`;

export const GET_ITEM = gql`
    query GetItem($id: String!) {
        GetItem(id: $id) {
            id,
            title,
            slug,
            summary,
            content
        }
    }
`;
