import React from 'react';
import { Button, Modal } from 'react-bootstrap';

import getResource from '../../../resourceHelper';

const InformationModal = props => {
    return (
        <Modal keyboard={false} backdrop="static"
            show={props.isShow} onHide={props.onClose}
            dialogClassName="custom-modal modal-md"
        >
            <Modal.Header closeButton>
                <Modal.Title>PA Focus Information</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <table className="table table-bordered table-striped">
                    <tbody>
                        <tr>
                            <td>Purpose of PA Focus</td>
                            <td>These data will help us evaluate every team member’s contribution and propose the bonus amount accordingly.</td>
                        </tr>
                        <tr>
                            <td />
                            <td />
                        </tr>
                        <tr>
                            <td>Applicable to promote</td>
                            <td>Is he/she eligible/ready for the Promotion at the end of this year?</td>
                        </tr>
                        <tr>
                            <td />
                            <td />
                        </tr>
                        <tr>
                            <td>Objective</td>
                            <td>Member's objectives.</td>
                        </tr>
                        <tr>
                            <td rowSpan="2">Other information</td>
                            <td>
                                This form is confidential and only applicable for team leaders to evaluate the team members.
                            </td>
                        </tr>
                        <tr>
                            <td>
                                The Management may have a face to face meeting with each team leader in order to clarify the information, if needed.
                                </td>
                        </tr>
                    </tbody>
                </table>
            </Modal.Body>
            <Modal.Footer>
                <Button className="btn-cancel" onClick={props.onClose}>{getResource('Close')}</Button>
            </Modal.Footer>
        </Modal>
    );
};

export default InformationModal;