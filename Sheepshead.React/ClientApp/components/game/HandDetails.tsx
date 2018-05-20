import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';

export interface HandDetailsState {
    gameId: string;
}

export default class HandDetails extends React.Component<any, any> {
    constructor(props: HandDetailsState) {
        super(props);
        //this.state = { gameId: IdUtils.getGameId(props) };
    }

    public render() {
        return (
            <div>
                <h4>Hand Details</h4>
            </div>
        );
    }
}