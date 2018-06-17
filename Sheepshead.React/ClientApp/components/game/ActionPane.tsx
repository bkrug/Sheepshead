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
    onTrickEnd: () => void;
}

export default class ActionPane extends React.Component<ActionPaneProps, ActionPaneState> {
    pickPhaseReached: boolean;
    buryPhaseReached: boolean;
    trickPhaseReached: boolean;

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
                playerCards: [],
                trickWinners: []
            },
            pickChoices: [],
            turnType: '',
        };
        this.onPickComplete = this.onPickComplete.bind(this);
        this.onBuryComplete = this.onBuryComplete.bind(this);
        this.onTrickPhaseComplete = this.onTrickPhaseComplete.bind(this);
        this.displayPhase = this.displayPhase.bind(this);
        this.loadPlayState = this.loadPlayState.bind(this);
        this.loadPlayState();
    }

    private loadPlayState(): void {
        var self = this;
        FetchUtils.get(
            'Game/GetPlayState?gameId=' + this.state.gameId + '&playerId=' + this.state.playerId,
            function (json: PlayState): void {
                switch (json.turnType) {
                    case 'Pick':
                        self.pickPhaseReached = true;
                        break;
                    case 'Bury':
                        self.buryPhaseReached = true;
                        break;
                    case 'PlayTrick':
                        self.trickPhaseReached = true;
                        break;
                }
                self.setState({
                    playState: json,
                    pickChoices: json.pickChoices,
                    turnType: json.turnType
                });
            });
    }

    private onPickComplete(): void {
        this.pickPhaseReached = false;
        this.buryPhaseReached = true;
        this.loadPlayState();
    }

    private onBuryComplete(): void {
        this.buryPhaseReached = false;
        this.trickPhaseReached = true;
        this.loadPlayState();
    }

    private onTrickPhaseComplete(): void {
        this.trickPhaseReached = false;
        this.loadPlayState();
    }

    private displayPhase(): string {
        if (this.pickPhaseReached && !this.trickPhaseReached)
            return 'Pick';
        if (this.buryPhaseReached && !this.pickPhaseReached)
            return 'Bury';
        if (this.trickPhaseReached && !this.buryPhaseReached)
            return 'PlayTrick';
        return '';
    }

    public selectRenderPhase() {
        var displayPhase = this.displayPhase();
        switch (displayPhase) {
            case 'Pick':
                return (<PickPane gameId={this.state.gameId}
                    pickChoices={this.state.pickChoices}
                    playerCards={this.state.playState.playerCards}
                    requestingPlayerTurn={this.state.playState.requestingPlayerTurn}
                    onPick={this.onPickComplete} />);
            case 'Bury':
                return (<BuryPane gameId={this.state.gameId}
                    playerCards={this.state.playState.playerCards}
                    requestingPlayerTurn={this.state.playState.requestingPlayerTurn}
                    onBury={this.onBuryComplete} />);
            case 'PlayTrick':
                return (<TrickPane gameId={this.state.gameId}
                    playerCards={this.state.playState.playerCards}
                    onTrickEnd={this.props.onTrickEnd}
                    onTrickPhaseComplete={this.onTrickPhaseComplete}
                    trickCount={6} playerCount={5} />);
            case 'BeginDeck':
                return (<div>Begin Deck Phase</div>);
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