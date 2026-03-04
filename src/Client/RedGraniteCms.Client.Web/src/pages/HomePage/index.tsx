import styled from "styled-components";
import { ListPagesTable } from "../../features/pages/listPages/listPagesTable";

interface IHomePageProps {
}

const Container = styled.div`
    width: 100%;
    height: 100%;
    display: flex;
    flex-direction: column;
    align-items: center;
`;

export function HomePage(props: IHomePageProps) {
    return (
        <Container>
            <ListPagesTable />
        </Container>
    );
}