import React, { PureComponent } from 'react';
import { Button, Modal } from 'react-bootstrap';

import getResource from '../../../resourceHelper';
import CustomScrollbars from '../../../shared/components/CustomScrollbars';
import { PerformanceProcess } from '../../../constants';

export default class CommentModal extends PureComponent {
    getTitle = () => {
        switch (this.props.processId) {
            case PerformanceProcess.Self: return getResource('SelfComment');
            case PerformanceProcess.Evaluator: return getResource('EvaluatorComment');
            case PerformanceProcess.Reviewer: return getResource('ReviewerComment');
        }
    }

    render() {
        return (
            <Modal show={this.props.showModal} onHide={this.props.onCloseModal}
                dialogClassName="custom-modal modal-lg comment-modal" backdrop="static"
            >
                <Modal.Header>
                    <Modal.Title>
                        {this.getTitle()}
                    </Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <CustomScrollbars>
                        <p className="comment">
                            {this.props.content}
                        </p>
                    </CustomScrollbars>
                </Modal.Body>
                <Modal.Footer>
                    <Button className="btn-cancel" onClick={this.props.onCloseModal}>{getResource('Close')}</Button>
                </Modal.Footer>
            </Modal>
        );
    }
}