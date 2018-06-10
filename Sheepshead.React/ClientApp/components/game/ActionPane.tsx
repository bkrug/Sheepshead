import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';
import { FetchUtils } from '../FetchUtils';
import { render } from 'react-dom';
import DraggableCard from './DraggableCard';
import PickPane from './PickPane';
import BuryPane from './BuryPane';
import TrickPane from './TrickPane';
import { PlayState, PickChoice } from './PlayState';

export interface ActionPaneState {
    gameId: string;
    playerId: string;
    playState: PlayState;
    pickChoices: any[];
    turnType: string;
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
                cardsPlayed: [],
                playerCards: []
            },
            pickChoices: [],
            turnType: '',
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
                    pickChoices: json.pickChoices,
                    turnType: json.turnType
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
        if (this.state.turnType == 'Pick')
            return 'Pick';
        if (this.state.turnType == 'Bury')
            return 'Bury';
        if (this.state.turnType == 'PlayTrick')
            return 'PlayTrick';
        else
            return 'Other';
    }

    public selectRenderPhase() {
        var displayPhase = this.displayPhase();
        switch (displayPhase) {
            case 'Pick':
                return (<PickPane gameId={this.state.gameId}
                    pickChoices={this.state.pickChoices}
                    playerCards={this.state.playState.playerCards}
                    requestingPlayerTurn={this.state.playState.requestingPlayerTurn}
                    onPick={this.loadPlayState} />);
            case 'Bury':
                return (<BuryPane gameId={this.state.gameId}
                    playerCards={this.state.playState.playerCards}
                    requestingPlayerTurn={this.state.playState.requestingPlayerTurn}
                    onBury={this.loadPlayState} />);
            case 'PlayTrick':
                return (<TrickPane gameId={this.state.gameId}
                    playerCards={this.state.playState.playerCards} />);
            default:
                return (
                    <div>
                        <h4>Other Phase</h4>
                        This is not a Pick or Bury or Trick phase.
                    </div>);
        }
    }

    public render() {
        return (
            <div>
                { this.selectRenderPhase() }
            </div>
        );
    }
}