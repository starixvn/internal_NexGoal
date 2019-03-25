import React, { PureComponent } from 'react';
import { Button, Modal } from 'react-bootstrap';

import getResource from '../../resourceHelper';

export default class PerformanceGradeInfo extends PureComponent {
    renderGradeInfo = (grade) =>
        <tr key={grade.gradeId}>
            <td>{grade.name}</td>
            <td>{grade.description}</td>
            <td><p>{grade.explanation}</p></td>
        </tr>

    render() {
        return (
            <Modal show={this.props.showModal} onHide={this.props.onCloseModal} bsSize="lg"
                dialogClassName="custom-modal" backdrop="static"
            >
                <Modal.Header>
                    <Modal.Title>
                        Performance Grade Explanation
                        </Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <table className="table table-bordered table-striped">
                        <thead>
                            <tr>
                                <th>{getResource('Grade')}</th>
                                <th>{getResource('GradeName')}</th>
                                <th>{getResource('Explanation')}</th>
                            </tr>
                        </thead>
                        <tbody>
                            {this.props.performanceGradeInfos.map(this.renderGradeInfo)}
                        </tbody>
                    </table>
                </Modal.Body>
                <Modal.Footer>
                    <Button className="btn-cancel" onClick={this.props.onCloseModal}>{getResource('Close')}</Button>
                </Modal.Footer>
            </Modal>
        );
    }
}