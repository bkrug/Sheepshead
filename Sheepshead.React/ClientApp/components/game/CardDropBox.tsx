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
    canDrop?: boolean
    isOver?: boolean
    connectDropTarget?: ConnectDropTarget
}

const boxTarget = {
    drop() {
        return { name: 'DropBox' }
    },
}

@DropTarget(
    'card',
    boxTarget,
    (connect: DropTargetConnector, monitor: DropTargetMonitor) => ({
        connectDropTarget: connect.dropTarget(),
        isOver: monitor.isOver(),
        canDrop: monitor.canDrop(),
    }),
)

export default class CardDropBox extends React.Component<CardDropBoxProps, any> {
    public static propTypes = {
        connectDragSource: PropTypes.func.isRequired,
        isDragging: PropTypes.bool.isRequired,
        card: PropTypes.string.isRequired,
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