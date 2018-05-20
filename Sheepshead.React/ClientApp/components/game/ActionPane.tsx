import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';

export interface ActionPaneState {
    gameId: string;
}

export default class ActionPane extends React.Component<any, any> {
    constructor(props: ActionPaneState) {
        super(props);
        //this.state = { gameId: IdUtils.getGameId(props) };
    }

    public render() {
        return (
            <div>
                <h4>Action Pane Details</h4>
            </div>
        );
    }
}