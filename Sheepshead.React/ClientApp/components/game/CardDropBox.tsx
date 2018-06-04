import * as React from 'react';
import * as PropTypes from 'prop-types';
import {
    DropTarget,
    DropTargetConnector,
    DropTargetMonitor,
    ConnectDropTarget,
} from 'react-dnd';

const style: React.CSSProperties = {
    height: '12rem',
    width: '12rem',
    marginRight: '1.5rem',
    marginBottom: '1.5rem',
    color: 'white',
    padding: '1rem',
    textAlign: 'center',
    fontSize: '1rem',
    lineHeight: 'normal',
    float: 'left',
}

export interface CardDropBoxProps {
    accepts: string[]
    canDrop?: boolean
    lastDroppedItem?: any
    isOver?: boolean
    connectDropTarget?: ConnectDropTarget
    onDrop: (item: any) => void
}

const boxTarget = {
    drop(props: CardDropBoxProps, monitor: DropTargetMonitor) {
        console.log(props);
        props.onDrop(monitor.getItem());
    },
}

@DropTarget(
    (props: CardDropBoxProps) => props.accepts,
    boxTarget,
    (connect: DropTargetConnector, monitor: DropTargetMonitor) => ({
        connectDropTarget: connect.dropTarget(),
        isOver: monitor.isOver(),
        canDrop: monitor.canDrop(),
    }),
)

export default class CardDropBox extends React.Component<CardDropBoxProps> {
    public static propTypes = {
        connectDropTarget: PropTypes.func.isRequired,
        isOver: PropTypes.bool.isRequired,
        canDrop: PropTypes.bool.isRequired,
        accepts: PropTypes.arrayOf(PropTypes.string).isRequired,
        lastDroppedItem: PropTypes.object,
        onDrop: PropTypes.func.isRequired,
    }

    public render() {
        const { canDrop, isOver, connectDropTarget } = this.props
        const isActive = canDrop && isOver

        let backgroundColor = '#222'
        if (isActive) {
            backgroundColor = 'darkgreen'
        } else if (canDrop) {
            backgroundColor = 'darkkhaki'
        }

        if (connectDropTarget)
            return (
                connectDropTarget(<div style={{ ...style, backgroundColor }} >{isActive ? 'Release to drop' : 'Drag a box here'}</div>)
            )
        else
            return (<div></div>)
    }
}