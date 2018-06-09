import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';
import { FetchUtils } from '../FetchUtils';
import { render } from 'react-dom';
import DraggableCard from './DraggableCard';
import PickPane from './PickPane';
import BuryPane from './BuryPane';
import { PlayState, PickChoice } from './PlayState';

export interface ActionPaneState {
    gameId: string;
    playerId: string;
    playState: PlayState;
    pickChoices: any[];
}

export interface ActionPaneProps extends React.Props<any> {
    gameId: string;
}

export default class ActionPane extends React.Component<ActionPaneProps, ActionPaneState> {
    constructor(props: ActionPaneProps) {
        super(props);
        this.state = {
            gameId: props.gameId,
            playerId: IdUtils.getPlayerId(props.gameId) || '',
            playState: {
                turnType: '',
                humanTurn: false,
                requestingPlayerTurn: false,
                blinds: [],
                pickChoices: [],
                cardsPlayed: {},
                playerCards: []
            },
            pickChoices: []
        };
        this.loadPlayState = this.loadPlayState.bind(this);
        this.loadPlayState();
    }

    private loadPlayState(): void {
        var self = this;
        FetchUtils.repeatGet(
            'Game/GetPlayState?gameId=' + this.state.gameId + '&playerId=' + this.state.playerId,
            function (json: PlayState): void {
                self.setState({
                    playState: json,
                    pickChoices: json.pickChoices
                });
            },
            function (json: PlayState): boolean {
                return false;
            },
            1000);
    }

    private displayPhase(): string {
        if (this.state.playState == null)
            return 'Other';
        if (this.state.playState.turnType == 'Pick')
            return 'Pick';
        //if (this.state.playState.turnType == 'Bury' && !this.state.playState.requestingPlayerTurn)
        //    return 'Pick';
        if (this.state.playState.turnType == 'Bury') // && this.state.playState.requestingPlayerTurn)
            return 'Bury';
        else
            return 'Other';
    }

    public render() {
        return (
            <div>
                {
                    this.displayPhase() == 'Pick'
                        ? <PickPane gameId={this.state.gameId}
                            pickChoices={this.state.pickChoices}
                            playerCards={this.state.playState.playerCards}
                            requestingPlayerTurn={this.state.playState.requestingPlayerTurn}
                            onPick={this.loadPlayState} />
                        :
                    this.displayPhase() == 'Bury'
                        ? <BuryPane gameId={this.state.gameId}
                            playerCards={this.state.playState.playerCards}
                            requestingPlayerTurn={this.state.playState.requestingPlayerTurn}
                            onBury={this.loadPlayState} />
                    : <div>
                                <h4>Other Phase</h4>
                                This is not a Pick or Bury phase.
                      </div>
                }
            </div>
        );
    }
}