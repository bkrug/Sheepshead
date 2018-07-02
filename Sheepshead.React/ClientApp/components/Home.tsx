import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { withRouter } from 'react-router-dom';

export class Home extends React.Component<RouteComponentProps<{}>, {}> {
    constructor(props: any) {
        super(props);
        window.location.href = '/setup/create';
    }

    public render() {
        return <div>
            Redirecting to the game setup page.
        </div>;
    }
}