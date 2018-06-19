﻿import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';
import { FetchUtils } from '../FetchUtils';
import { render } from 'react-dom';
import DraggableCard from './DraggableCard';
import PickPane from './PickPane';
import BuryPane from './BuryPane';
import TrickPane from './TrickPane';
import HandSummaryPane from './HandSummaryPane';
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
    onHandEnd: () => void;
}

export default class ActionPane extends React.Component<ActionPaneProps, ActionPaneState> {
    phaseToDisplay: string;

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
        this.onSummaryPhaseComplete = this.onSummaryPhaseComplete.bind(this);
        this.loadPlayState = this.loadPlayState.bind(this);
        this.loadPlayState();
    }

    private loadPlayState(): void {
        var self = this;
        FetchUtils.get(
            'Game/GetPlayState?gameId=' + this.state.gameId + '&playerId=' + this.state.playerId,
            function (json: PlayState): void {
                self.phaseToDisplay = json.turnType == 'BeginDeck' ? 'ReportHand' : json.turnType;
                self.setState({
                    playState: json,
                    pickChoices: json.pickChoices,
                    turnType: json.turnType
                });
            });
    }

    private onPickComplete(): void {
        this.loadPlayState();
    }

    private onBuryComplete(): void {
        this.loadPlayState();
    }

    private onTrickPhaseComplete(): void {
        this.loadPlayState();
    }

    private onSummaryPhaseComplete(): void {
        this.props.onHandEnd();
        this.loadPlayState();
        var self = this;
        FetchUtils.get(
            'Game/StartDeck?gameId=' + this.state.gameId + '&playerId=' + this.state.playerId,
            function (json: PlayState): void {
                self.phaseToDisplay = json.turnType == 'BeginDeck' ? 'ReportHand' : json.turnType;
                self.setState({
                    playState: json,
                    pickChoices: json.pickChoices,
                    turnType: json.turnType
                });
            });
    }

    public selectRenderPhase() {
        var displayPhase = this.phaseToDisplay;
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
            case 'ReportHand':
                return (<HandSummaryPane gameId={this.state.gameId}
                    onSummaryPhaseComplete={this.onSummaryPhaseComplete} />);
            case 'BeginDeck':
                return (<div>Begin Deck Phase</div>);
            default:
                return (
                    <div>
                        <h4>Other Phase</h4>
                        This is not a recognized phase.
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