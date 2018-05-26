import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';
import { FetchUtils } from '../FetchUtils';

export interface CardsHeldPaneState {
    gameId: string;
    playerId: string;
}

export default class CardsHeldPane extends React.Component<any, any> {
    constructor(props: CardsHeldPaneState) {
        super(props);
        this.state = {
            gameId: props.gameId,
            playerId: IdUtils.getPlayerId(props.gameId)
        };
        var self = this;
        FetchUtils.get(
            'Game/GetCards?gameId=' + this.state.gameId + "&playerId=" + this.state.playerId,
            function (json: any): void {
                console.log(json);
                //self.setState({ scores: json });
            }
        );
    }

    public render() {
        return (
            <div>
                <h4>Cards Held Pane</h4>
            </div>
        );
    }
}