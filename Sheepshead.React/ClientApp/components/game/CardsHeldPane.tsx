import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';

export interface CardsHeldPaneState {
    gameId: string;
}

export default class CardsHeldPane extends React.Component<any, any> {
    constructor(props: CardsHeldPaneState) {
        super(props);
        //this.state = { gameId: IdUtils.getGameId(props) };
    }

    public render() {
        return (
            <div>
                <h4>Cards Held Pane</h4>
            </div>
        );
    }
}