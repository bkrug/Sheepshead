import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';
import { FetchUtils } from '../FetchUtils';

export interface ActionPaneState {
    gameId: string;
    playerId: string;
    turnType: string;
    humanTurn: boolean;
    requestingPlayerTurn: boolean;
    blinds: number[];
    pickChoices: { [key: string]: boolean };
    cardsPlayed: { [key: string]: string };
}

export default class ActionPane extends React.Component<any, any> {
    constructor(props: ActionPaneState) {
        super(props);
        this.state = {
            gameId: props.gameId,
            playerId: IdUtils.getPlayerId(props.gameId)
        };
        var self = this;
        FetchUtils.get(
            'Game/GetPlayState?gameId=' + this.state.gameId + '&playerId=' + this.state.playerId,
            function (json: any): void {
                self.setState({
                    turnType: json.turnType,
                    humanTurn: json.humanTurn,
                    requestingPlayerTurn: json.requestingPlayerTurn,
                    blinds: json.blinds,
                    pickChoices: json.pickChoices,
                    cardsPlayed: json.cardsPlayed
                });
            }
        );
    }

    private renderPick() {
        return (
            <div>
                <b>Pick</b>
                {
                    this.state.pickChoices.map(
                        (pickChoice: any, i: number) =>
                            <div key={i}>
                                <p>{pickChoice.item1 + (pickChoice.item2 ? ' picked.' : ' refused.')}</p>
                            </div>
                    )
                }
                <div>
                {
                    this.state.requestingPlayerTurn ? <b>Do you want to pick?</b> : ''
                }
                </div>
            </div>
        );
    }

    public render() {
        return (
            <div>
                <h4>Action Pane Details</h4>
                {
                    this.state.turnType == 'Pick'
                        ? this.renderPick()
                        : <div>Other</div>
                }
            </div>
        );
    }
}