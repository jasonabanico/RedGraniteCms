import { ListPagesTable } from "../../features/pages/listPages/listPagesTable";

interface IHomePageProps {
}

export function HomePage(props: IHomePageProps) {
    return (
        <div className="w-full h-full flex flex-col items-center">
            <ListPagesTable />
        </div>
    );
}
