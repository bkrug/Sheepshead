﻿import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';
import { FetchUtils } from '../FetchUtils';
import { render } from 'react-dom';
import PickPane from './PickPane';
import BuryPane from './BuryPane';
import TrickPane from './TrickPane';
import HandSummaryPane from './HandSummaryPane';
import { PlayState, PickChoice } from './PlayState';

export interface ActionPaneState {
    gameId: string;
    playerId: string;
    turnType: string;
    trickCount: number;
    playerCount: number;
    gameExists: boolean;
}

export interface ActionPaneProps extends React.Props<any> {
    gameId: string;
    onTrickEnd: () => void;
    onHandEnd: () => void;
}

export interface GameType {
    turnType: string;
    trickCount: number;
    playerCount: number;
    gameExists: boolean;
}

export default class ActionPane extends React.Component<ActionPaneProps, ActionPaneState> {
    constructor(props: ActionPaneProps) {
        super(props);
        this.state = {
            gameId: props.gameId,
            playerId: IdUtils.getPlayerId(props.gameId) || '',
            turnType: '',
            trickCount: 0,
            playerCount: 0,
            gameExists: true
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
            'Game/GetTurnType?gameId=' + this.state.gameId + '&playerId=' + this.state.playerId,
            function (json: GameType): void {
                self.setState({
                    turnType: json.turnType == 'BeginHand' ? 'ReportHand' : json.turnType,
                    playerCount: json.playerCount,
                    trickCount: json.trickCount,
                    gameExists: json.gameExists
                });
                if (json.turnType == 'PlayTrick')
                    self.props.onTrickEnd();
            });
    }

    private onPickComplete(mustRedeal: boolean): void {
        if (mustRedeal)
            this.setState({ turnType: 'ReportHand' });
        else
            this.setState({ turnType: 'Bury' });
    }

    private onBuryComplete(): void {
        this.setState({ turnType: 'PlayTrick' });
        this.props.onTrickEnd();
    }

    private onTrickPhaseComplete(): void {
        this.setState({ turnType: 'ReportHand' });
    }

    private onSummaryPhaseComplete(): void {
        var self = this;
        this.props.onHandEnd();
        FetchUtils.post(
            'Game/StartDeck?gameId=' + this.state.gameId + '&playerId=' + this.state.playerId,
            function (json: PlayState): void { self.setState({ turnType: json.turnType }); });
    }

    public selectRenderPhase() {
        switch (this.state.turnType) {
            case 'Pick':
                return (<PickPane
                    gameId={this.state.gameId}
                    onPick={this.onPickComplete} />);
            case 'Bury':
                return (<BuryPane
                    gameId={this.state.gameId}
                    onBury={this.onBuryComplete} />);
            case 'PlayTrick':
                return (<TrickPane
                    gameId={this.state.gameId}
                    onTrickEnd={this.props.onTrickEnd}
                    onTrickPhaseComplete={this.onTrickPhaseComplete}
                    trickCount={this.state.trickCount}
                    playerCount={this.state.playerCount} />);
            case 'ReportHand':
            case 'BeginHand':
                return (<HandSummaryPane
                    gameId={this.state.gameId}
                    onSummaryPhaseComplete={this.onSummaryPhaseComplete} />);
            default:
                return (
                    <div>
                        <h4>Other Phase</h4>
                        { this.state.gameExists ? 'This is not a recognized phase.' : 'A game with this identifier does not exist.' }
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