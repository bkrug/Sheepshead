import * as React from 'react';
import { CardSummary } from 'ClientApp/components/game/PlayState';

export interface CardProps {
    cardSummary: CardSummary,
    onClick?: (card: DraggableCard) => void
}

export interface CardState {
    illegalNotification: boolean;
}

export default class DraggableCard extends React.Component<CardProps, CardState> {
    constructor(props: CardProps) {
        super(props);
        this.state = {
            illegalNotification: false
        };
        this.onClick = this.onClick.bind(this);
    }

    private onClick(e: any): void {
        if (this.props.cardSummary.legalMove == false) {
            this.setState({ illegalNotification: true });
            var self = this;
            setTimeout(function () { self.setState({ illegalNotification: false }); }, 3000);
        }
        else if (this.props.onClick)
            this.props.onClick(this);
    }

    public render() {
        const { filename } = this.props.cardSummary;
        if (this.state.illegalNotification)
            return (
                <div style={{ height: '96px', width: '72px', backgroundColor: 'red', textAlign: 'center', display: 'inline-block' }} >ILLEGAL MOVE</div>
            )
        else
            return (
                <img src={'./img/' + filename + '.png'} alt={filename} onClick={this.onClick} style={{ cursor: "pointer" }}/>
            )
	}
}