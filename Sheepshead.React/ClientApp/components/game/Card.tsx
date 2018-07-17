import * as React from 'react';
import { CardSummary } from 'ClientApp/components/game/PlayState';

export interface CardProps {
    cardSummary: CardSummary,
    onClick?: (card: Card) => void
}

export interface CardState {
    illegalNotification: boolean;
}

export default class Card extends React.Component<CardProps, CardState> {
    constructor(props: CardProps) {
        super(props);
        this.state = {
            illegalNotification: false
        };
        this.onClick = this.onClick.bind(this);
        this.userCouldClick = this.userCouldClick.bind(this);
    }

    private userCouldClick(): boolean {
        return this.props.onClick ? true : false;
    }

    private onClick(e: any): void {
        if (this.userCouldClick() && this.props.cardSummary.legalMove == false) {
            this.setState({ illegalNotification: true });
            var self = this;
            setTimeout(function () { self.setState({ illegalNotification: false }); }, 500);
        }
        else if (this.props.onClick)
            this.props.onClick(this);
    }

    public render() {
        const { filename } = this.props.cardSummary;
        var cursorType = this.userCouldClick() ? 'pointer' : 'not-allowed';
        if (this.state.illegalNotification)
            return (
                <img src={'./img/illegal.png'} alt={'illegal move'} style={{ cursor: cursorType }} />
            )
        else
            return (
                <img src={'./img/' + filename + '.png'} alt={filename} onClick={this.onClick} style={{ cursor: cursorType }}/>
            )
	}
}