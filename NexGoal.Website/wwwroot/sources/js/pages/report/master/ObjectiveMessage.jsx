import React, { PureComponent, Fragment } from 'react';
import { Button, Modal } from 'react-bootstrap';

import getResource from '../../../resourceHelper';

export default class ObjectiveMessage extends PureComponent {
    render() {
        return (
            <Modal show={this.props.showModal} onHide={this.props.onCloseModal}
                dialogClassName="custom-modal modal-lg objective-message-modal"
            >
                <Modal.Header>
                    <Modal.Title>
                        {getResource('Objectives')}
                    </Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    {this.props.record &&
                        <Fragment>
                            <h5>{getResource('CurrentObjectives')}</h5>
                            <p className="comment">
                                {this.props.record.currentObjective}
                            </p>

                            <h5>{getResource('NextObjectives')}</h5>
                            <p className="comment">
                                {this.props.record.nextObjective}
                            </p>
                        </Fragment>
                    }

                </Modal.Body>
                <Modal.Footer>
                    <Button className="btn-cancel" onClick={this.props.onCloseModal}>{getResource('Close')}</Button>
                </Modal.Footer>
            </Modal>
        );
    }
}