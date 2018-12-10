import React, { PureComponent, Fragment } from 'react';
import { Button, Modal, Tab, Nav, NavItem } from 'react-bootstrap';

import getResource from '../../resourceHelper';

export default class NominatorComment extends PureComponent {
    constructor(props) {
        super(props);
    }

    componentDidMount() {
        this.tabNode = document.getElementById('tabs');
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
        return (
            <Modal backdrop="static" show={this.props.showModal} onHide={this.props.onCloseModal}
                dialogClassName="custom-modal modal-lg"
            >
                <Modal.Header>
                    <Modal.Title>{getResource('Comment')}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Tab.Container id="tab-container" defaultActiveKey={1}>
                        <Fragment>
                            <Nav id="tabs" bsStyle="tabs">
                                <NavItem eventKey={1} >
                                    NavItem 1 content
                                </NavItem>
                                <NavItem eventKey={2}>
                                    NavItem 2 content
                                </NavItem>
                                <NavItem eventKey={2}>
                                    NavItem 2 content
                                </NavItem>
                                <NavItem eventKey={2}>
                                    NavItem 2 content
                                </NavItem>
                                <NavItem eventKey={2}>
                                    NavItem 2 content
                                </NavItem>
                                <NavItem eventKey={2}>
                                    NavItem 2 content
                                </NavItem>
                                <NavItem eventKey={2}>
                                    NavItem 2 content
                                </NavItem>
                                <NavItem eventKey={2}>
                                    NavItem 2 content
                                </NavItem>
                                <NavItem eventKey={2}>
                                    NavItem 2 content
                                </NavItem>
                                <NavItem eventKey={2}>
                                    NavItem 2 content
                                </NavItem>
                                <NavItem eventKey={2}>
                                    NavItem 2 content
                                </NavItem>
                                <NavItem eventKey={2}>
                                    NavItem 2 content
                                </NavItem>
                                <NavItem eventKey={2}>
                                    NavItem 2 content
                                </NavItem>

                            </Nav>
                            <Tab.Content>
                                <Tab.Pane eventKey={1}>
                                    131231231
                                </Tab.Pane>
                                <Tab.Pane eventKey={2}>
                                    131231231231231213
                                </Tab.Pane>
                            </Tab.Content>
                        </Fragment>
                    </Tab.Container>

                </Modal.Body>
                <Modal.Footer>
                    <Button className="btn-cancel" onMouseDown={this.handleScrollLeft} onMouseUp={this.handleMouseUp}>left</Button>
                    <Button className="btn-cancel" onMouseDown={this.handleScrollRight} onMouseUp={this.handleMouseUp}>right</Button>
                    <Button className="btn-cancel" onClick={this.props.onCloseModal}>{getResource('Close')}</Button>
                </Modal.Footer>
            </Modal>
        );
    }
}