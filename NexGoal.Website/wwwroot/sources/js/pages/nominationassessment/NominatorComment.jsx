import React, { PureComponent, Fragment } from 'react';
import { Button, Modal, Tab, Nav, NavItem } from 'react-bootstrap';

import getResource from '../../resourceHelper';

export default class NominatorComment extends PureComponent {
    constructor(props) {
        super(props);
    }

    handleScrollLeft = () => {
        this.scrolllInterval = setInterval((element) => {
            element.scrollTo(element.scrollLeft - 10, 0);
        }, 50, document.getElementById('tabs'));
    }

    handleScrollRight = () => {
        this.scrolllInterval = setInterval((element) => {
            element.scrollTo(element.scrollLeft + 10, 0);
        }, 50, document.getElementById('tabs'));
    }

    handleMouseUp = () => {
        clearInterval(this.scrolllInterval);
    }

    render() {
        if (this.props.nomination) {
            const nominators = this.props.nomination.nominators;

            return (
                <Modal backdrop="static" show={this.props.showModal} onHide={this.props.onCloseModal}
                    dialogClassName="custom-modal modal-lg nomination-comment-modal"
                >
                    <Modal.Header>
                        <Modal.Title>Votes for {`${this.props.nomination.nominee.fullName} (${this.props.nomination.nominee.nickName})`} - {this.props.period.title}</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <Tab.Container id="tab-container" defaultActiveKey={this.props.activeKey}>
                            <Fragment>
                                <div className="nav-tabs-wrapper">
                                    <span className="icon-chevron-left" onMouseDown={this.handleScrollLeft} onMouseUp={this.handleMouseUp}></span>
                                    <Nav id="tabs" bsStyle="tabs">
                                        {nominators.map(n =>
                                            <NavItem key={n.performanceRecordId} eventKey={n.performanceRecordId} >
                                                {n.creator.nickName}
                                            </NavItem>
                                        )}
                                    </Nav>
                                    <span className="icon-chevron-right" onMouseDown={this.handleScrollRight} onMouseUp={this.handleMouseUp}></span>
                                </div>
                                <Tab.Content>
                                    {nominators.map(n =>
                                        <Tab.Pane key={n.performanceRecordId} eventKey={n.performanceRecordId}>
                                            <p className="comment">{n.comment}</p>
                                        </Tab.Pane>
                                    )}
                                </Tab.Content>
                            </Fragment>
                        </Tab.Container>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button className="btn-cancel" onClick={this.props.onCloseModal}>{getResource('Close')}</Button>
                    </Modal.Footer>
                </Modal>
            );
        }

        return '';
    }
}